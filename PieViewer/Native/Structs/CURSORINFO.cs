using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PieViewer.Native.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct CURSORINFO
{
    /// <summary>
    /// Specifies the size, in bytes, of the structure. 
    /// </summary>
    public int cbSize;

    /// <summary>
    /// Specifies the cursor state. This parameter can be one of the following values:
    /// </summary>
    public int flags;

    ///<summary>
    ///Handle to the cursor. 
    ///</summary>
    public nint hCursor;

    /// <summary>
    /// A POINT structure that receives the screen coordinates of the cursor. 
    /// </summary>
    public POINT ptScreenPos;
}