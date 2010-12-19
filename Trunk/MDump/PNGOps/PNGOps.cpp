#include <png.h>
#include <cstdlib>
#include "PNGOps.h"

static char* MagicString = "MDmpMrge";
static const size_t kPngSigLen = 8;
static const int kBitDepth = 8;

extern "C"
{
	__declspec(dllexport) MergedCode __cdecl IsMergedImage(char* filename)
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
			return MC_ERROR;
		}

		if(fread(rBuff, 1, kPngSigLen, fp) != kPngSigLen)
		{
			free(rBuff);
			//There weren't even 8 bytes in the file.
			//Any image would have at least 8
			return MC_ERROR;
		}
		if(png_sig_cmp(rBuff, 0, kPngSigLen))
		{
			//Not a PNG
			free(rBuff);
			return MC_NOT_MERGED;
		}
		free(rBuff);

		png_structp readStruct = png_create_read_struct(PNG_LIBPNG_VER_STRING,
			nullptr, nullptr, nullptr);
		if(readStruct == nullptr)
		{
			return MC_ERROR;
		}

		png_infop info = png_create_info_struct(readStruct);
		if(info == nullptr)
		{
			png_destroy_read_struct(&readStruct, nullptr, nullptr);
			return MC_ERROR;
		}

		if(setjmp(png_jmpbuf(readStruct)))
		{
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
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

		fclose(fp);
		png_destroy_read_struct(&readStruct, &info, nullptr);
		return ret;
	}


	__declspec(dllexport) ECode __cdecl LoadMergedImage(char* filename, png_bytepp bitmapOut,
		int* widthOut, int* heightOut, char** mdDataOut, int* mdDataLenOut)
	{
		//Set initial value of out arguments to null or 0, so if the function errors out, they have
		//this value on exit
		*bitmapOut = nullptr;
		*mdDataOut = nullptr;
		*widthOut = *heightOut = *mdDataLenOut = 0;

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
			return EC_INIT_FAILURE;
		}

		png_infop info = png_create_info_struct(readStruct);
		if(info == nullptr)
		{
			png_destroy_read_struct(&readStruct, nullptr, nullptr);
			return EC_INIT_FAILURE;
		}

		if(setjmp(png_jmpbuf(readStruct)))
		{
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_IO_FAILURE;
		}
		png_init_io(readStruct, fp);

		if(setjmp(png_jmpbuf(readStruct)))
		{
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_RW_INFO_FAILURE;
		}
		png_read_info(readStruct, info);

		//Make sure our image 32-bpp
		if(png_get_bit_depth(readStruct, info) != kBitDepth
			|| png_get_color_type(readStruct, info) != PNG_COLOR_TYPE_RGBA)
		{
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_BAD_FORMAT;
		}

		png_textp textInfo;
		int numTextFields;
		png_get_text(readStruct, info, &textInfo, &numTextFields);

		//MDump files only have one text field and the key is the magic string
		if(numTextFields != 1
			 || strcmp(textInfo->key, MagicString) != 0)
		{
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_RW_INFO_FAILURE;
		}
		//Get mdump data and copy it to a buffer
		int mdDataLen = textInfo[0].text_length;
		char* mdData = static_cast<char*>(malloc(mdDataLen));
		if(mdData == nullptr)
		{
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_ALLOC_FAILURE;
		}
		memcpy(mdData, textInfo[0].text, mdDataLen);

		int height = png_get_image_height(readStruct, info);
		int width = png_get_image_width(readStruct, info);
		int rowLen = width * 4;
		
		//Split image into rows
		png_bytep bitmap = static_cast<png_bytep>(malloc(rowLen * height));
		if(bitmap == nullptr)
		{
			free(mdData);
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_ALLOC_FAILURE;
		}

		png_bytepp rowPointers = static_cast<png_bytepp>(malloc(height * sizeof(png_bytep)));
		if(rowPointers == nullptr)
		{
			free(mdData);
			free(bitmap);
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_ALLOC_FAILURE;
		}

		png_bytep rowStart = bitmap;
		for(int c = 0; c < height; ++c)
		{
			rowPointers[c] = rowStart;
			rowStart += rowLen;
		}

		//Read in the image
		if(setjmp(png_jmpbuf(readStruct)))
		{
			free(mdData);
			free(bitmap);
			fclose(fp);
			png_destroy_read_struct(&readStruct, &info, nullptr);
			return EC_RW_IMAGE_FAILURE;
		}
		png_read_image(readStruct, rowPointers);


		//Assign the out arguments with their values
		*mdDataLenOut = mdDataLen;
		*mdDataOut = mdData;
		*bitmapOut = bitmap;
		//Close up shop
		fclose(fp);
		png_destroy_read_struct(&readStruct, &info, nullptr);
		return EC_SUCCESS;
	}

	__declspec(dllexport) ECode __cdecl SavePNG(unsigned char* bitmap, int width, int height, char* filename,
		bool flipRGB, char* mdData, int mdDataLen)
	{
			if(bitmap == nullptr || width < 0 || height < 0 || filename == nullptr
				|| mdData == nullptr || mdDataLen == 0)
		{
			return EC_BAD_ARGS;
		}

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

		FILE* fp = fopen(filename, "wb");
		if(!fp)
		{
			png_destroy_write_struct(&writeStruct, &info);
			return EC_IO_FAILURE;
		}
		//TEMP: Swap out for memory functions once we get this working
		if(setjmp(png_jmpbuf(writeStruct)))
		{
			fclose(fp);
			png_destroy_write_struct(&writeStruct, &info);
			return EC_IO_FAILURE;
		}
		png_init_io(writeStruct, fp);

		if(setjmp(png_jmpbuf(writeStruct)))
		{
			fclose(fp);
			png_destroy_write_struct(&writeStruct, &info);
			return EC_INIT_FAILURE;
		}
		//Use maximum compression level
		png_set_compression_level(writeStruct, 9);
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
			fclose(fp);
			png_destroy_write_struct(&writeStruct, &info);
			return EC_ALLOC_FAILURE;
		}

		memset(textInfo, 0, sizeof(png_text));

		if(setjmp(png_jmpbuf(writeStruct)))
		{
			free(textInfo);
			fclose(fp);
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
			fclose(fp);
			png_destroy_write_struct(&writeStruct, &info);
			return EC_HEADER_FAILURE;
		}
		png_set_IHDR(writeStruct, info, width, height,
			kBitDepth, PNG_COLOR_TYPE_RGBA, PNG_INTERLACE_NONE,
			PNG_COMPRESSION_TYPE_DEFAULT, PNG_FILTER_TYPE_DEFAULT);
		png_write_info(writeStruct, info);


		//Split image into rows
		int rowLen = width * 4;
		png_bytepp rowPointers = static_cast<png_bytepp>(malloc(height * sizeof(png_bytep)));
		if(rowPointers == nullptr)
		{
			free(textInfo);
			fclose(fp);
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
			fclose(fp);
			png_destroy_write_struct(&writeStruct, &info);
			return EC_RW_IMAGE_FAILURE;
		}
		png_write_image(writeStruct, rowPointers); 

		if(setjmp(png_jmpbuf(writeStruct)))
		{
			free(textInfo);
			free(rowPointers);
			fclose(fp);
			png_destroy_write_struct(&writeStruct, &info);
			return EC_RW_END_FAILURE;
		}
		png_write_end(writeStruct, info);

		//TEMP: Swap out file i/o for mem i/o later
		free(textInfo);
		free(rowPointers);
		fclose(fp);
		png_destroy_write_struct(&writeStruct, &info);
		return EC_SUCCESS;
	}
}