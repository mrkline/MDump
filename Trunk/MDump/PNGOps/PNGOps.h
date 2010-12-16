#pragma once

enum ECode
{
	EC_SUCCESS,
	EC_BAD_ARGS,
	EC_INIT_FAILURE,
	EC_IO_FAILURE,
	EC_HEADER_FAILURE,
	EC_ALLOC_FAILURE,
	EC_WRITE_IMAGE_FAILURE,
	EC_WRITE_INFO_FAILURE,
	EC_WRITE_END_FAILURE
};

extern "C"
{
	__declspec(dllexport) void FreeBitmap(void* ptr) { free(ptr); }

	__declspec(dllexport) bool __cdecl IsPNG(char* filename);

	__declspec(dllexport) ECode __cdecl LoadPNG(char* filename, unsigned char** bitmapOut, int* widthOut, int* heightOut);

	//PNG is assumed to be in 32-bpp RBGA format
	__declspec(dllexport) ECode __cdecl SavePNG(unsigned char* bitmap, int width, int height, char* filename,
		bool flipRGB, char* mdData, int mdDataLen);
}