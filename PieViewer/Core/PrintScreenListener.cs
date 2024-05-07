using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PieViewer.Core
{
    /// <summary>
    /// A low level keyboard global hook that only listen for the print screen keyboard key down
    /// </summary>
    internal sealed partial class PrintScreenListener
    {
        /// <summary>
        /// Hook ID
        /// </summary>
        private IntPtr hookID = IntPtr.Zero;


        /// <summary>
        /// Internal callback processing function
        /// </summary>
        private delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        private KeyboardHookHandler? hookHandler;


        #region Events

        public delegate void PrintScreenCallback();
        public event PrintScreenCallback? PrintScreen;

        public delegate Task PrintScreenAsyncCallback();
        public event PrintScreenAsyncCallback? PrintScreenAsync;

        #endregion Events


        #region Management 

        /// <summary>
        /// Install low level keyboard hook
        /// </summary>
        public void Install()
        {
            hookHandler = HookFunc;
            hookID = SetHook(hookHandler);
        }

        /// <summary>
        /// Remove low level keyboard hook
        /// </summary>
        public void Uninstall()
        {
            UnhookWindowsHookEx(hookID);
        }

        #endregion Management



        /// <summary>
        /// Registers hook with Windows API
        /// </summary>
        /// <param name="proc">Callback function</param>
        /// <returns>Hook ID</returns>
        private IntPtr SetHook(KeyboardHookHandler proc)
        {
            using ProcessModule? module = Process.GetCurrentProcess().MainModule;
            if (module is not null)
                return SetWindowsHookEx(13, proc, GetModuleHandle(module.ModuleName), 0);
            return IntPtr.Zero;
        }

        /// <summary>
        /// Default hook call, which fire the PrintScreen event asynchronously when the print screen key is pressed
        /// This asynchronous call allow for CallNextHookEx to be called before the event handler and prevent blocking the Windows loop (maybe...)
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int iwParam = wParam.ToInt32();
                if (iwParam == WM_KEYDOWN && Marshal.ReadInt32(lParam) == VKEY_SNAPSHOT)
                {
                    Task.Run(() => { PrintScreenAsync?.Invoke(); });
                    PrintScreen?.Invoke();
                }
                    
                //{
                //    var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
                //    Task.Factory.StartNew(() => { PrintScreenAsync?.Invoke(); }, CancellationToken.None, TaskCreationOptions.None, uiContext);
                //}
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }




        /// <summary>
        /// Low-Level function declarations
        /// </summary>
        #region WinAPI

        private const int WM_KEYDOWN = 0x100;       // Keyboard KeyDown message
        private const int VKEY_SNAPSHOT = 0x2C;     // PRINT SCREEN key

        [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
        private static partial IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler lpfn, IntPtr hMod, uint dwThreadId);

        [LibraryImport("user32.dll", EntryPoint = "UnhookWindowsHookEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool UnhookWindowsHookEx(IntPtr hhk);

        [LibraryImport("user32.dll", EntryPoint = "CallNextHookEx", SetLastError = true)]
        private static partial IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        private static partial IntPtr GetModuleHandle(string lpModuleName);

        #endregion WinAPI
    }
}
