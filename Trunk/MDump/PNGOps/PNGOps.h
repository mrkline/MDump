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
	__declspec(dllexport) ECode __cdecl SavePNGToMemory(png_bytep bitmap, int width, int height,
		bool flipRGB, char* mdData, int mdDataLen, png_bytepp memPngOut, int* memPngLenOut);
}

//Writes to a given memory buffer, then chops off unneeded data on completion.
class MemoryWriter
{
public:
	MemoryWriter(png_bytep buff);
	void Write(png_bytep data, png_size_t len);
	png_bytep TrimBuff();
	png_size_t GetBytesWritten() { return bytesWritten; }
	static void WriteCallback(png_structp png_ptr, png_bytep data, png_size_t length);
	static void FlushCallback(png_structp png_ptr) {}
	
private:
	png_bytep startingByte;
	png_bytep currentByte;
	png_size_t bytesWritten;
	static MemoryWriter* instance;
};

