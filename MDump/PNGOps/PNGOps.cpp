#include <png.h>
#include <new>
#include <memory>
#include <cstdlib>
#include "PNGOps.h"

static char* MagicString = "MDmpMrge";
static const size_t kPngSigLen = 8;
static const int kBitDepth = 8;
static const int kBytesPerPix = 4;

extern "C"
{
	__declspec(dllexport) MergedCode EXP_CALL_CONV IsMergedImage(char* filename)
	{
		FILE* fp = fopen(filename, "rb");
		if(fp == nullptr)
		{
			return MC_ERROR;
		}

		//Read the first 8 bytes (the size of the png signiature)
		//and check to see if it's a PNG
		png_byte* rBuff = static_cast<unsigned char*>(malloc(8));
		if(rBuff == nullptr)
		{
			fclose(fp);
			return MC_ERROR;
		}

		if(fread(rBuff, 1, kPngSigLen, fp) != kPngSigLen)
		{
			free(rBuff);
			fclose(fp);
			//There weren't even 8 bytes in the file.
			//Any image would have at least 8
			return MC_ERROR;
		}
		if(png_sig_cmp(rBuff, 0, kPngSigLen))
		{
			//Not a PNG
			free(rBuff);
			fclose(fp);
			return MC_NOT_MERGED;
		}
		free(rBuff);

		png_structp readStruct = png_create_read_struct(PNG_LIBPNG_VER_STRING,
			nullptr, nullptr, nullptr);
		if(readStruct == nullptr)
		{
			fclose(fp);
			return MC_ERROR;
		}

		png_infop info = png_create_info_struct(readStruct);
		if(info == nullptr)
		{
			png_destroy_read_struct(&readStruct, nullptr, nullptr);
			fclose(fp);
			return MC_ERROR;
		}

		if(setjmp(png_jmpbuf(readStruct)))
		{
			png_destroy_read_struct(&readStruct, &info, nullptr);
			fclose(fp);
			return MC_ERROR;
		}
		png_init_io(readStruct, fp);
		png_set_sig_bytes(readStruct, kPngSigLen);
		png_read_info(readStruct, info);

		png_textp textInfo;
		int numTextFields;

		png_get_text(readStruct, info, &textInfo, &numTextFields);

		MergedCode ret = MC_NOT_MERGED;

		//MDump files only have one text field and the key is the magic string
		if(numTextFields == 1
			&& strcmp(textInfo->key, MagicString) == 0)
		{
			ret = MC_MERGED;
		}

		png_destroy_read_struct(&readStruct, &info, nullptr);
		fclose(fp);
		return ret;
	}


	__declspec(dllexport) ECode EXP_CALL_CONV LoadMergedImageData(char* filename,
		char** mdDataOut, int* mdDataLenOut)
	{
		//Set initial value of out arguments to null or 0, so if the function errors out, they have
		//this value on exit
		*mdDataOut = nullptr;
		*mdDataLenOut = 0;

		FILE* fp = fopen(filename, "rb");
		if(fp == nullptr)
		{
			return EC_IO_FAILURE;
		}

		//Assume we've already checked to make sure that image is a png

		png_structp readStruct = png_create_read_struct(PNG_LIBPNG_VER_STRING,
			nullptr, nullptr, nullptr);
		if(readStruct == nullptr)
		{
			fclose(fp);
			return EC_INIT_FAILURE;
		}

		png_infop info = png_create_info_struct(readStruct);
		if(info == nullptr)
		{
			png_destroy_read_struct(&readStruct, nullptr, nullptr);
			fclose(fp);
			return EC_INIT_FAILURE;
		}

		if(setjmp(png_jmpbuf(readStruct)))
		{
			png_destroy_read_struct(&readStruct, &info, nullptr);
			fclose(fp);
			return EC_IO_FAILURE;
		}
		png_init_io(readStruct, fp);

		if(setjmp(png_jmpbuf(readStruct)))
		{
			png_destroy_read_struct(&readStruct, &info, nullptr);
			fclose(fp);
			return EC_RW_INFO_FAILURE;
		}
		png_read_info(readStruct, info);

		png_textp textInfo;
		int numTextFields;
		png_get_text(readStruct, info, &textInfo, &numTextFields);

		//MDump files only have one text field and the key is the magic string
		if(numTextFields != 1
			 || strcmp(textInfo->key, MagicString) != 0)
		{
			png_destroy_read_struct(&readStruct, &info, nullptr);
			fclose(fp);
			return EC_RW_INFO_FAILURE;
		}
		//Get mdump data and copy it to a buffer
		png_size_t mdDataLen = textInfo[0].text_length;
		char* mdData = static_cast<char*>(malloc(mdDataLen));
		if(mdData == nullptr)
		{
			png_destroy_read_struct(&readStruct, &info, nullptr);
			fclose(fp);
			return EC_ALLOC_FAILURE;
		}
		memcpy(mdData, textInfo[0].text, mdDataLen);
				

		//Assign the out arguments with their values
		*mdDataLenOut = mdDataLen;
		*mdDataOut = mdData;
		//Close up shop
		png_destroy_read_struct(&readStruct, &info, nullptr);
		fclose(fp);
		return EC_SUCCESS;
	}

	__declspec(dllexport) ECode EXP_CALL_CONV SavePNGToMemory(png_bytep bitmap, int width, int height,
		bool flipRGB, char* mdData, int mdDataLen, int compLevel,  png_bytepp memPngOut, int* memPngLenOut)
	{
		//Set initial value of out arguments to null or 0, so if the function errors out, they have
		//this value on exit
		*memPngOut = nullptr;
		*memPngLenOut = 0;

		if(bitmap == nullptr || width < 0 || height < 0
					|| mdData == nullptr || mdDataLen == 0)
			{
				return EC_BAD_ARGS;
			}

		png_bytep outBuff = nullptr;
		try
		{
			//Create a buffer the size of the bitmap (no way it will be too small to fit the compressed PNG).
			outBuff = static_cast<png_bytep>(malloc(width * height * kBytesPerPix));
			if(outBuff == nullptr)
			{
				return EC_ALLOC_FAILURE;
			}

			std::auto_ptr<MemoryWriter> mw(new MemoryWriter(outBuff));

			png_structp writeStruct = png_create_write_struct(PNG_LIBPNG_VER_STRING,
				nullptr, nullptr, nullptr);
			if(writeStruct == nullptr)
			{
				return EC_INIT_FAILURE;
			}

			png_infop info = png_create_info_struct(writeStruct);
			if(info == nullptr)
			{
				png_destroy_write_struct(&writeStruct, nullptr);
				return EC_INIT_FAILURE;
			}

			if(setjmp(png_jmpbuf(writeStruct)))
			{
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_IO_FAILURE;
			}
			png_set_write_fn(writeStruct, mw.get(), MemoryWriter::WriteCallback, MemoryWriter::FlushCallback);

			if(setjmp(png_jmpbuf(writeStruct)))
			{
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_INIT_FAILURE;
			}
			//Use maximum compression level
			png_set_compression_level(writeStruct, compLevel);
			//Set compression strategy
			png_set_compression_strategy(writeStruct, Z_FILTERED);
			//Use all filters
			png_set_filter(writeStruct, 0, PNG_ALL_FILTERS);

			if(flipRGB)
			{
				//Swap RGB to BGR (since the bitmaps appear to be supplied in this manner)
				png_set_bgr(writeStruct);
			}

			//Set up MDump info
			png_textp textInfo = static_cast<png_textp>(malloc(sizeof(png_text)));
			if(textInfo == nullptr)
			{
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_ALLOC_FAILURE;
			}

			memset(textInfo, 0, sizeof(png_text));

			if(setjmp(png_jmpbuf(writeStruct)))
			{
				free(textInfo);
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_RW_INFO_FAILURE;
			}
			textInfo->compression = PNG_TEXT_COMPRESSION_zTXt;
			textInfo->text_length = mdDataLen;
			textInfo->text = mdData;
			textInfo->key = MagicString;
			png_set_text(writeStruct, info, textInfo, 1);

			//Write header
			if(setjmp(png_jmpbuf(writeStruct)))
			{
				free(textInfo);
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_HEADER_FAILURE;
			}
			png_set_IHDR(writeStruct, info, width, height,
				kBitDepth, PNG_COLOR_TYPE_RGBA, PNG_INTERLACE_NONE,
				PNG_COMPRESSION_TYPE_DEFAULT, PNG_FILTER_TYPE_DEFAULT);
			png_write_info(writeStruct, info);


			//Split image into rows
			int rowLen = width * kBytesPerPix;
			png_bytepp rowPointers = static_cast<png_bytepp>(malloc(height * sizeof(png_bytep)));
			if(rowPointers == nullptr)
			{
				free(textInfo);
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_ALLOC_FAILURE;
			}

			png_bytep rowStart = bitmap;
			for(int c = 0; c < height; ++c)
			{
				rowPointers[c] = rowStart;
				rowStart += rowLen;
			}

			if(setjmp(png_jmpbuf(writeStruct)))
			{
				free(textInfo);
				free(rowPointers);
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_RW_IMAGE_FAILURE;
			}
			png_write_image(writeStruct, rowPointers); 

			if(setjmp(png_jmpbuf(writeStruct)))
			{
				free(textInfo);
				free(rowPointers);
				free(outBuff);
				png_destroy_write_struct(&writeStruct, &info);
				return EC_RW_END_FAILURE;
			}
			png_write_end(writeStruct, info);

			//TEMP: Swap out file i/o for mem i/o later
			free(textInfo);
			free(rowPointers);
			png_destroy_write_struct(&writeStruct, &info);
			*memPngLenOut = mw->GetBytesWritten();
			*memPngOut = mw->TrimBuff(); //Set the output to the trimmed buffer
			return EC_SUCCESS;
		}
		catch(std::bad_alloc)
		{
			free(outBuff);
			return EC_ALLOC_FAILURE;
		}
	}
}

MemoryWriter::MemoryWriter(png_bytep buff)
{
	startingByte = currentByte = buff;
	bytesWritten = 0;
}

void MemoryWriter::Write(png_bytep data, png_size_t len)
{
	memcpy(currentByte, data, len);
	bytesWritten += len;
	currentByte += len;
}

png_bytep MemoryWriter::TrimBuff()
{
	return static_cast<png_bytep>(realloc(startingByte, bytesWritten));
}

void MemoryWriter::WriteCallback(png_structp png_ptr, png_bytep data, png_size_t length)
{
	MemoryWriter* instance = static_cast<MemoryWriter*>(png_ptr->io_ptr);
	instance->Write(data, length);
}