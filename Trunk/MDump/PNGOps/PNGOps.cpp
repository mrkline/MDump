#include <png.h>
#include <cstdlib>
#include "PNGOps.h"

static char* MagicString = "MDmpMrge";

extern "C"
{
	__declspec(dllexport) bool __cdecl IsPNG(char* filename)
	{
		return false;
	}


	__declspec(dllexport) ECode __cdecl LoadPNG(char* filename, unsigned char** bitmapOut, int* widthOut, int* heightOut)
	{
		FILE* fp = fopen(filename, "rb");
		if(fp == nullptr)
		{
			return EC_IO_FAILURE;
		}


		fclose(fp);
		return EC_SUCCESS;
	}

	__declspec(dllexport) ECode __cdecl SavePNG(unsigned char* bitmap, int width, int height, char* filename,
		bool flipRGB, char* mdData, int mdDataLen)
	{
			if(bitmap == nullptr || width < 0 || height < 0 || filename == nullptr)
		{
			return EC_BAD_ARGS;
		}

		png_structp pngStruct = png_create_write_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL);
		if(pngStruct == NULL)
		{
			return EC_INIT_FAILURE;
		}

		png_infop info = png_create_info_struct(pngStruct);
		if(info == NULL)
		{
			png_destroy_write_struct(&pngStruct, NULL);
			return EC_INIT_FAILURE;
		}

		FILE* fp = fopen(filename, "wb");
		if(!fp)
		{
			png_destroy_write_struct(&pngStruct, &info);
			return EC_IO_FAILURE;
		}
		//TEMP: Swap out for memory functions once we get this working
		if(setjmp(png_jmpbuf(pngStruct)))
		{
			fclose(fp);
			png_destroy_write_struct(&pngStruct, &info);
			return EC_IO_FAILURE;
		}
		png_init_io(pngStruct, fp);

		if(setjmp(png_jmpbuf(pngStruct)))
		{
			fclose(fp);
			png_destroy_write_struct(&pngStruct, &info);
			return EC_INIT_FAILURE;
		}
		//Use maximum compression level
		png_set_compression_level(pngStruct, 9);
		//Set compression strategy
		png_set_compression_strategy(pngStruct, Z_FILTERED);
		//Use all filters
		png_set_filter(pngStruct, 0, PNG_ALL_FILTERS);

		if(flipRGB)
		{
			//Swap RGB to BGR (since the bitmaps appear to be supplied in this manner)
			png_set_bgr(pngStruct);
		}

		//Set up MDump info
		png_textp textInfo = nullptr;
		if(mdDataLen > 0)
		{
			textInfo = static_cast<png_textp>(malloc(sizeof(png_textp)));
			if(textInfo == nullptr)
			{
				fclose(fp);
				png_destroy_write_struct(&pngStruct, &info);
				return EC_ALLOC_FAILURE;
			}

			memset(textInfo, 0, sizeof(png_text));
	
			if(setjmp(png_jmpbuf(pngStruct)))
			{
				free(textInfo);
				fclose(fp);
				png_destroy_write_struct(&pngStruct, &info);
				return EC_WRITE_INFO_FAILURE;
			}
			textInfo->compression = PNG_TEXT_COMPRESSION_zTXt;
			textInfo->text_length = mdDataLen;
			textInfo->text = mdData;
			textInfo->key = MagicString;
			png_set_text(pngStruct, info, textInfo, 1);
		}

		//Write header
		if(setjmp(png_jmpbuf(pngStruct)))
		{
			free(textInfo);
			fclose(fp);
			png_destroy_write_struct(&pngStruct, &info);
			return EC_HEADER_FAILURE;
		}
		png_set_IHDR(pngStruct, info, width, height,
			8, PNG_COLOR_TYPE_RGBA, PNG_INTERLACE_NONE,
			PNG_COMPRESSION_TYPE_DEFAULT, PNG_FILTER_TYPE_DEFAULT);
		png_write_info(pngStruct, info);


		//Split image into rows
		int rowLen = width * 4;
		png_bytepp rowPointers = static_cast<png_bytepp>(malloc(height * sizeof(png_bytep)));
		if(rowPointers == nullptr)
		{
			free(textInfo);
			fclose(fp);
			png_destroy_write_struct(&pngStruct, &info);
			return EC_ALLOC_FAILURE;
		}

		png_bytep rowStart = bitmap;
		for(int c = 0; c < height; ++c)
		{
			rowPointers[c] = rowStart;
			rowStart += rowLen;
		}

		if(setjmp(png_jmpbuf(pngStruct)))
		{
			free(textInfo);
			free(rowPointers);
			fclose(fp);
			png_destroy_write_struct(&pngStruct, &info);
			return EC_WRITE_IMAGE_FAILURE;
		}
		png_write_image(pngStruct, rowPointers); 

		if(setjmp(png_jmpbuf(pngStruct)))
		{
			free(textInfo);
			free(rowPointers);
			fclose(fp);
			png_destroy_write_struct(&pngStruct, &info);
			return EC_WRITE_END_FAILURE;
		}
		png_write_end(pngStruct, info);

		//TEMP: Swap out file i/o for mem i/o later
		free(textInfo);
		free(rowPointers);
		fclose(fp);
		png_destroy_write_struct(&pngStruct, &info);
		return EC_SUCCESS;
	}
}