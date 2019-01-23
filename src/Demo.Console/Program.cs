using Barcode.Generator;
using Barcode.Generator.Common;
using Barcode.Generator.Rendering;
using System;
using System.IO;

namespace Demo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the image with ZXing.NET.RenderOnly
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 10,
                    PureBarcode = true
                }
            };

            // Write text and generate a 2-D barcode as a bitmap
            PixelData barcodeImage = barcodeWriter.Write("Welcome to Barcode Generator!");
            byte[] bmpBytes = BitmapConverter.FromPixelData(barcodeImage);

            // save bitmap as file
            
            File.WriteAllBytes($"{Environment.CurrentDirectory}/../../../Generated images/image.bmp", bmpBytes);
        }
    }
}
