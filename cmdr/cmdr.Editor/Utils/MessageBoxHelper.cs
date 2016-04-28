using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace cmdr.Editor.Utils
{
    static class MessageBoxHelper
    {
        static MessageBoxHelper()
        {
            // center message boxes
            MessageBoxHelper.PrepToCenterMessageBoxOnForm(App.Current.MainWindow);
        }

        public static void ShowInfo(string message, string title = null)
        {
            MessageBox.Show(App.Current.MainWindow, message, title ?? "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarning(string message, string title = null)
        {
            MessageBox.Show(App.Current.MainWindow, message, title ?? "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string message, string title = null)
        {
            MessageBox.Show(App.Current.MainWindow, message, title ?? "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ShowQuestion(string message, string title = null)
        {
            return MessageBox.Show(App.Current.MainWindow, message, title ?? "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static bool? ShowCancellableQuestion(string message, string title = null)
        {
            var result = MessageBox.Show(App.Current.MainWindow, message, title ?? "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }


        #region MessageBoxCenterHelper by Linda Lawton

        /// <summary>
        /// The following code ist taken from Linda Lawton http://www.daimto.com/wpf-center-message-box-on-window/
        /// </summary>
        /// <param name="form"></param>

        internal static void PrepToCenterMessageBoxOnForm(Window form)
        {
            MessageBoxCenterHelper helper = new MessageBoxCenterHelper();
            helper.Prep(form);
        }

        private class MessageBoxCenterHelper
        {
            private int messageHook;
            private IntPtr parentFormHandle;

            public void Prep(Window form)
            {
                NativeMethods.CenterMessageCallBackDelegate callBackDelegate = new NativeMethods.CenterMessageCallBackDelegate(CenterMessageCallBack);
                GCHandle.Alloc(callBackDelegate);

                parentFormHandle = new WindowInteropHelper(form).Handle;
                messageHook = NativeMethods.SetWindowsHookEx(5, callBackDelegate, new IntPtr(NativeMethods.GetWindowLong(parentFormHandle, -6)), NativeMethods.GetCurrentThreadId()).ToInt32();
            }

            private int CenterMessageCallBack(int message, int wParam, int lParam)
            {
                NativeMethods.RECT formRect;
                NativeMethods.RECT messageBoxRect;
                int xPos;
                int yPos;

                if (message == 5)
                {
                    NativeMethods.GetWindowRect(parentFormHandle, out formRect);
                    NativeMethods.GetWindowRect(new IntPtr(wParam), out messageBoxRect);

                    xPos = (int)((formRect.Left + (formRect.Right - formRect.Left) / 2) - ((messageBoxRect.Right - messageBoxRect.Left) / 2));
                    yPos = (int)((formRect.Top + (formRect.Bottom - formRect.Top) / 2) - ((messageBoxRect.Bottom - messageBoxRect.Top) / 2));

                    NativeMethods.SetWindowPos(wParam, 0, xPos, yPos, 0, 0, 0x1 | 0x4 | 0x10);
                    NativeMethods.UnhookWindowsHookEx(messageHook);
                }

                return 0;
            }
        }

        private static class NativeMethods
        {
            internal struct RECT
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            internal delegate int CenterMessageCallBackDelegate(int message, int wParam, int lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool UnhookWindowsHookEx(int hhk);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("kernel32.dll")]
            internal static extern int GetCurrentThreadId();

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr SetWindowsHookEx(int hook, CenterMessageCallBackDelegate callback, IntPtr hMod, int dwThreadId);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        }

        # endregion
    }
}
