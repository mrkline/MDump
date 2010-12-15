#include <png.h>
#include <stdlib.h>
#include "PNGOps.h"

extern "C" __declspec(dllexport) ECode __cdecl SavePNG(unsigned char* bitmap, int width, int height, char* filename,
	bool flipRGB)
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

	//TEMP: Swap out for memory functions once we get this working
	if(setjmp(png_jmpbuf(pngStruct)))
	{
		png_destroy_write_struct(&pngStruct, &info);
		return EC_IO_FAILURE;
	}
	FILE* fp = fopen(filename, "wb");
	if(!fp)
	{
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
	//Swap RGB to BGR (since the bitmaps appear to be supplied in this manner)
	png_set_bgr(pngStruct);


	//Write header
	if(setjmp(png_jmpbuf(pngStruct)))
	{
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
		free(rowPointers);
		fclose(fp);
		png_destroy_write_struct(&pngStruct, &info);
		return EC_WRITE_IMAGE_FAILURE;
	}
	png_set_rows(pngStruct, info, rowPointers);
	png_write_image(pngStruct, rowPointers);

	//TODO: Insert text writing here
	if(setjmp(png_jmpbuf(pngStruct)))
	{
		free(rowPointers);
		fclose(fp);
		png_destroy_write_struct(&pngStruct, &info);
		return EC_WRITE_END_FAILURE;
	}
	png_write_end(pngStruct, info);

	//TEMP: Swap out later
	free(rowPointers);
	fclose(fp);
	png_destroy_write_struct(&pngStruct, &info);
	return EC_SUCESSS;
}