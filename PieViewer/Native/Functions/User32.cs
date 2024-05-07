using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using PieViewer.Native.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


[assembly: DisableRuntimeMarshalling]

namespace PieViewer.Native.Functions;

internal static partial class User32
{
    [LibraryImport("user32.dll", EntryPoint = "GetCursorInfo")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorInfo(out CURSORINFO pci);

    [LibraryImport("user32.dll", EntryPoint = "CopyIcon")]
    public static partial nint CopyIcon(nint hIcon);

    //[DllImport("user32.dll", EntryPoint = "GetIconInfo")]
    //public static extern bool GetIconInfo(nint hIcon, out ICONINFO piconinfo);

    [LibraryImport("user32.dll", EntryPoint = "GetIconInfo")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetIconInfo(nint hIcon, out ICONINFO piconinfo);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DrawIconEx(nint hdc, int xLeft, int yTop, nint hIcon, int cxWidth, int cyHeight, int istepIfAniCur, nint hbrFlickerFreeDraw, int diFlags);
}
