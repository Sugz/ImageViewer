using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
//using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Runtime.InteropServices;
using PieViewer.Native;
using PieViewer.Native.Structs;
using PieViewer.Native.Functions;
using System.Reflection.Metadata;
using System.Diagnostics;

namespace PieViewer.Core;

internal sealed class ScreenCapture
{
    private Rectangle _desktopRect = Rectangle.Empty;

    private void InitDesktopRect()
    {
        foreach (Screen screen in Screen.AllScreens)
            _desktopRect = Rectangle.Union(_desktopRect, screen.Bounds);
    }

    public Bitmap CaptureDesktopAsBitmap()
    {
        Bitmap bmp = new(_desktopRect.Width, _desktopRect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        using Graphics graphics = Graphics.FromImage(bmp);
        graphics.CopyFromScreen(_desktopRect.Location, Point.Empty, bmp.Size);

        return bmp;
    }

    private static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap? bitmap)
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

    public BitmapSource CaptureDesktop()
    {
        return CreateBitmapSourceFromGdiBitmap(CaptureDesktopAsBitmap());
    }

    public static BitmapSource CaptureImageCursor(ref Point point)
    {
        return CreateBitmapSourceFromGdiBitmap(CaptureImageCursorAsBitmap(ref point));
    }

    

    public ScreenCapture()
    {
        InitDesktopRect();
    }


    public static unsafe Bitmap? CaptureImageCursorAsBitmap(ref Point point)
    {
        try
        {
            CURSORINFO cursorInfo = new() { cbSize = sizeof(CURSORINFO) };

            if (!User32.GetCursorInfo(out cursorInfo))
                return null;

            if (cursorInfo.flags != Constants.CURSOR_SHOWING)
                return null;

            nint hicon = User32.CopyIcon(cursorInfo.hCursor);
            if (hicon == nint.Zero)
                return null;

            if (!User32.GetIconInfo(hicon, out ICONINFO iconInfo))
            {
                Gdi32.DeleteObject(hicon);
                return null;
            }

            point.X = cursorInfo.ptScreenPos.X - iconInfo.xHotspot;
            point.Y = cursorInfo.ptScreenPos.Y - iconInfo.yHotspot;

            using (Bitmap maskBitmap = Image.FromHbitmap(iconInfo.hbmMask))
            {
                //Is this a monochrome cursor?  
                if (maskBitmap.Height == maskBitmap.Width * 2 && iconInfo.hbmColor == nint.Zero)
                {
                    Bitmap final = new(maskBitmap.Width, maskBitmap.Width);
                    using (Graphics resultGraphics = Graphics.FromImage(final))
                    {
                        nint resultHdc = resultGraphics.GetHdc();
                        User32.DrawIconEx(resultHdc, 0, 0, cursorInfo.hCursor, 0, 0, 0, nint.Zero, 0x0003);
                        resultGraphics.ReleaseHdc(resultHdc);
                    }

                    Gdi32.DeleteObject(iconInfo.hbmMask);

                    return final;
                }

                Gdi32.DeleteObject(iconInfo.hbmColor);
                Gdi32.DeleteObject(iconInfo.hbmMask);
                Gdi32.DeleteObject(hicon);
            }

            return Icon.FromHandle(hicon).ToBitmap(); ;
        }
        catch (Exception ex)
        {
            //You should catch exception with your method here.
            Debug.WriteLine(ex, "Impossible to get the cursor.");
        }

        return null;
    }
}
