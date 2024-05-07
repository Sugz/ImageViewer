using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PieViewer.Core
{
    internal static class BitmapHelper
    {
        public static Bitmap ApplyMask(Bitmap input, Bitmap mask)
        {
            Bitmap output = new(input.Width, input.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            output.MakeTransparent();
            var rect = new Rectangle(0, 0, input.Width, input.Height);

            var bitsMask = mask.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitsInput = input.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitsOutput = output.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < input.Height; y++)
                {
                    byte* ptrMask = (byte*)bitsMask.Scan0 + y * bitsMask.Stride;
                    byte* ptrInput = (byte*)bitsInput.Scan0 + y * bitsInput.Stride;
                    byte* ptrOutput = (byte*)bitsOutput.Scan0 + y * bitsOutput.Stride;
                    for (int x = 0; x < input.Width; x++)
                    {
                        //I think this is right - if the blue channel is 0 than all of them are (monochrome mask) which makes the mask black
                        if (ptrMask[4 * x] == 0)
                        {
                            ptrOutput[4 * x] = ptrInput[4 * x]; // blue
                            ptrOutput[4 * x + 1] = ptrInput[4 * x + 1]; // green
                            ptrOutput[4 * x + 2] = ptrInput[4 * x + 2]; // red

                            //Ensure opaque
                            ptrOutput[4 * x + 3] = 255;
                        }
                        else
                        {
                            ptrOutput[4 * x] = 0; // blue
                            ptrOutput[4 * x + 1] = 0; // green
                            ptrOutput[4 * x + 2] = 0; // red

                            //Ensure Transparent
                            ptrOutput[4 * x + 3] = 0; // alpha
                        }
                    }
                }

            }
            mask.UnlockBits(bitsMask);
            input.UnlockBits(bitsInput);
            output.UnlockBits(bitsOutput);

            return output;
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
