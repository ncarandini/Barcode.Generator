/*
 * Copyright 2019 Nicolò Carandini
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
 using System;

namespace Barcode.Generator.Rendering
{
    public static class BitmapConverter
    {
        static readonly int BMP_HEADER_LENGHT = 14 + 40; // As a sum of the lenght of the Header first part and the lenght of the DIB Header part
        static readonly short BMP_HEADER_FIELD = 19778; // as Litte endian: 0x42 0x4D

        public static byte[] FromPixelData(PixelData pixelData)
        {
            if (pixelData == null)
            {
                throw new ArgumentNullException("PixelData can't be null");
            }

            byte[] bmpBytes = new byte[pixelData.Pixels.Length + BMP_HEADER_LENGHT];
            int writePointer = 0;

            // == HEADER ==

            // Write the Headerfield
            writePointer = WriteByteArray(ref bmpBytes, writePointer, BMP_HEADER_FIELD);

            // Write the size of the BMP file
            writePointer = WriteByteArray(ref bmpBytes, writePointer, bmpBytes.Length);

            // Write the two reserved (and tipically unused) values that we set as 0x00 0x00 0x00 0x00 (i.e. 0, whom defaul .NET type is int32)
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 0);

            // Write the offset, i.e. starting address, of the byte where the bitmap image data (pixel array) can be found
            writePointer = WriteByteArray(ref bmpBytes, writePointer, BMP_HEADER_LENGHT);

            // == DIB header (bitmap information header) of type BITMAPINFOHEADER ==

            // Write The size of this header
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 40);

            // Write the bitmap width in pixels(signed integer)
            writePointer = WriteByteArray(ref bmpBytes, writePointer, pixelData.Width);

            // Write the bitmap height in pixels(signed integer)
            writePointer = WriteByteArray(ref bmpBytes, writePointer, pixelData.Height);

            // Write the number of color planes (must be 1, as a signed short value)
            writePointer = WriteByteArray(ref bmpBytes, writePointer, (short)1);

            // Write the number of bits per pixel, which is the color depth of the image. here we use 32 because every pixel is 4 bytes long.
            writePointer = WriteByteArray(ref bmpBytes, writePointer, (short)32); 

            // Write the compression method being used. We don't use it so we set it to BI_RGB (None = 0)
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 0);

            // Write the image size. This is the size of the raw bitmap data; a dummy 0 can be given for BI_RGB bitmaps
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 0);

            // Write the horizontal resolution of the image. At 96 DPI * 39.3701 inches per metre = 3780 pixel per metre
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 3780);

            // Write the vertical resolution of the image. At 96 DPI * 39.3701 inches per metre = 3780 pixel per metre
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 3780);

            // Write the number of colors in the color palette, or 0 to default to 2n
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 0);

            // Write the number of important colors used, or 0 when every color is important; generally ignored
            writePointer = WriteByteArray(ref bmpBytes, writePointer, 0);

            // == BITMAP
            byte[] pixel = new byte[4];

            // We need to read from left to right and from bottom to top
            for (int rowIndex = pixelData.Height -1; rowIndex >= 0; rowIndex--)
            {
                for (int colIndex = 0; colIndex < pixelData.Width; colIndex++)
                {
                    int pixelStartIndex = (rowIndex * pixelData.Width + colIndex) * 4; // because every pixel is 4 bytes long

                    pixel[0] = pixelData.Pixels[pixelStartIndex];
                    pixel[1] = pixelData.Pixels[pixelStartIndex + 1];
                    pixel[2] = pixelData.Pixels[pixelStartIndex + 2];
                    pixel[3] = pixelData.Pixels[pixelStartIndex + 3];

                    // Write the pixel data
                    writePointer = WriteByteArray(ref bmpBytes, writePointer, pixel);
                }
            }
            

            // Return the result
            return bmpBytes;
        }

        static int WriteByteArray(ref byte[] byteArray, int index, byte value)
        {
            byteArray[index] = value;
            return index + 1;
        }

        static int WriteByteArray(ref byte[] byteArray, int index, short value)
        {
            var valueArray = BitConverter.GetBytes(value);
            Buffer.BlockCopy(valueArray, 0, byteArray, index, 2);
            return index + 2;
        }

        static int WriteByteArray(ref byte[] byteArray, int index, int value)
        {
            var valueArray = BitConverter.GetBytes(value);
            Buffer.BlockCopy(valueArray, 0, byteArray, index, 4);
            return index + 4;
        }

        static int WriteByteArray(ref byte[] byteArray, int index, byte[] pixelValue)
        {
            Buffer.BlockCopy(pixelValue, 0, byteArray, index, 4);
            return index + 4;
        }

    }
}
