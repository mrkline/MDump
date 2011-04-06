#pragma once

//Calling convention used for exported functions
#define EXP_CALL_CONV __cdecl

/*!
\brief Error codes to be returned from DLL functions.
\see MDump.PNGHandler.ECode
*/
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

/*!
\brief Return code for IsMergedImage
\see MDump.PNGHandler.MergedCode
*/
enum MergedCode
{
	MC_MERGED,
	MC_NOT_MERGED,
	MC_ERROR,
	MC_HAMMER //!< Completely necessary
};


extern "C"
{
	/*!
	\brief A simple wrapper around free() to allow managed code
			to free unmanaged memory once we've copied data out of it.
	\see MDump.PNGHandler.FreeUnmanagedData
	*/
	__declspec(dllexport) void EXP_CALL_CONV FreeUnmanagedData(void* ptr) { free(ptr); }

	/*!
	\brief  Checks if the image is merged by fist checking if it's a PNG, then if it contains the MDump metadata.
	\see MDump.PNGHandler.IsMergedImage
	*/
	__declspec(dllexport) MergedCode EXP_CALL_CONV IsMergedImage(char* filename);

	/*!
	\brief Loads an MDump data from a merged PNG.
	\see MDump.PNGHandler.LoadMergedImageData
	*/
	__declspec(dllexport) ECode EXP_CALL_CONV LoadMergedImageData(char* filename,
		char** mdDataOut, int* mdDataLenOut);

	/*!
	\brief Saves a PNG to unmanaged memory
	\see MDump.PNGHandler.SavePNGToMemory
	\warning PNG is assumed to be in 32-bpp RBGA format
	*/
	__declspec(dllexport) ECode EXP_CALL_CONV SavePNGToMemory(png_bytep bitmap, int width, int height,
		bool flipRGB, char* mdData, int mdDataLen, int compLevel, png_bytepp memPngOut, int* memPngLenOut);
}

/*!
\brief Simple RAII wrapper for C file handles.

C++ streams aren't used as libpng works with C file handles
*/
class FileReader
{
private:
	FILE* fp; //!< FILE handle pointer

public:
	FileReader(char* filepath);
	~FileReader();

	FILE* GetFilePointer() { return fp; }
};

//! Writes to a given memory buffer, then chops off unneeded data on completion.
class MemoryWriter
{
public:
	/*!
	\brief constructor which takes the buffer to write to
	\param buff Memory buffer to write to
	*/
	MemoryWriter(png_bytep buff);
	/*!
	\brief Copies bytes of data to the output buffer
	\param data Pointer to data to copy
	\param len Length of data to copy
	*/
	void Write(png_bytep data, png_size_t len);
	//! Reallocs the buffer to the length of the data written so far
	png_bytep TrimBuff();
	//! Gets the number of bytes copied via Write
	png_size_t GetBytesWritten() { return bytesWritten; }
	//! \brief Callback for libpng to write data.
	//! \see Write
	static void WriteCallback(png_structp png_ptr, png_bytep data, png_size_t length);
	//! Callback for lipng on flush. Does nothing.
	static void FlushCallback(png_structp png_ptr) {}
	
private:
	//! Pointer to the first byte in the output buffer
	png_bytep startingByte;
	//! Pointer to the current byte in the output buffer
	png_bytep currentByte;
	//! Number of bytes written via Write
	png_size_t bytesWritten;
};

