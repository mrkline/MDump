#pragma once

enum ECode
{
    EC_SUCCESS,
    EC_BAD_ARGS,
	EC_BAD_FORMAT,
    EC_INIT_FAILURE,
    EC_IO_FAILURE,
    EC_HEADER_FAILURE,
    EC_ALLOC_FAILURE,
    EC_RW_IMAGE_FAILURE,
    EC_RW_INFO_FAILURE,
    EC_RW_END_FAILURE
};

enum MergedCode
{
	MC_MERGED,
	MC_NOT_MERGED,
	MC_ERROR,
	MC_HAMMER //Completely necessary
};


extern "C"
{
	__declspec(dllexport) void FreeUnmanagedData(void* ptr) { free(ptr); }


	__declspec(dllexport) MergedCode __cdecl IsMergedImage(char* filename);

	__declspec(dllexport) ECode __cdecl LoadMergedImage(char* filename, png_bytepp bitmapOut,
		int* widthOut, int* heightOut, char** mdDataOut, int* mdDataLenOut);

	//PNG is assumed to be in 32-bpp RBGA format
	__declspec(dllexport) ECode __cdecl SavePNG(unsigned char* bitmap, int width, int height, char* filename,
		bool flipRGB, char* mdData, int mdDataLen);
}