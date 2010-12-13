#pragma once

enum ECode
{
	EC_SUCESSS,
	EC_BAD_ARGS,
	EC_INIT_FAILURE,
	EC_IO_FAILURE,
	EC_HEADER_FAILURE,
	EC_ALLOC_FAILURE,
	EC_WRITE_IMAGE_FAILURE,
	EC_WRITE_END_FAILURE
};

//PNG is assumed to be in 32-bpp RBGA format
extern "C" __declspec(dllexport) ECode __cdecl SavePNG(unsigned char* bitmap, int width, int height, char* filename);