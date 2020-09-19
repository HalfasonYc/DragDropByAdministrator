using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DragDropByAdministrator
{
    public class DragDropHelper
    {
        IntPtr dragDropHandle;

        public delegate void DragDropCallBackHandler(string text);

        DragDropCallBackHandler dragDropCallBackHandler;

        /// <summary>
        /// 注册拖放事件
        /// </summary>
        /// <param name="handle">窗口句柄，必须有效，检查控件IsHandleCreated</param>
        /// <param name="dragDropRetCallHandler">回调函数，不能为空</param>
        public void Resigter(IntPtr handle, DragDropCallBackHandler dragDropRetCallHandler)
        {
            this.dragDropHandle = handle;
            //此委托必须保持活动状态
            this.callBackHandler = new WndProcExHandler(this.WndProcEx);
            this.dragDropCallBackHandler = dragDropRetCallHandler;
            NativeMethods.ChangeWindowMessageFilterEx(this.dragDropHandle, 563, 1, 0);
            NativeMethods.ChangeWindowMessageFilterEx(this.dragDropHandle, 74, 1, 0);
            NativeMethods.ChangeWindowMessageFilterEx(this.dragDropHandle, 73, 1, 0);
            NativeMethods.DragAcceptFiles(this.dragDropHandle, true);
            this.lastCallPtr = NativeMethods.SetWindowLong(this.dragDropHandle, -4, Marshal.GetFunctionPointerForDelegate(this.callBackHandler));
        }

        public void UnResigter()
        {
            NativeMethods.SetWindowLong(this.dragDropHandle, -4, this.lastCallPtr);
            NativeMethods.DragAcceptFiles(this.dragDropHandle, false);
        }

        /// Return Type: LRESULT->LONG_PTR->int
        ///param0: HWND->HWND__*
        ///param1: UINT->unsigned int
        ///param2: WPARAM->UINT_PTR->unsigned int
        ///param3: LPARAM->LONG_PTR->int
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.StdCall)]
        private delegate IntPtr WndProcExHandler(IntPtr hwnd, int iMsg, IntPtr wParam, IntPtr lParam);

        private WndProcExHandler callBackHandler;
        private IntPtr lastCallPtr;

        private IntPtr WndProcEx(IntPtr hwnd, int iMsg, IntPtr wParam, IntPtr lParam)
        {
            if (iMsg == 563 && hwnd == this.dragDropHandle)
            {
                const uint buffer = 1000;
                StringBuilder bufferBuilder = new StringBuilder(1000), resultBuilder = new StringBuilder();
                uint n = NativeMethods.DragQueryFile(wParam, uint.MaxValue, null, 0);
                for (uint i = 0; i < n - 1; i++)
                {
                    NativeMethods.DragQueryFile(wParam, i, bufferBuilder, buffer);
                    resultBuilder.AppendLine(bufferBuilder.ToString());
                }
                NativeMethods.DragQueryFile(wParam, n - 1, bufferBuilder, buffer);
                resultBuilder.Append(bufferBuilder.ToString());
                this.dragDropCallBackHandler(resultBuilder.ToString());
                NativeMethods.DragFinish(wParam);
                return IntPtr.Zero;
            }
            return NativeMethods.CallWindowProc(this.lastCallPtr, hwnd, iMsg, wParam, lParam);
        }


        #region P/Invoke

        public partial class NativeMethods
        {
            [DllImport("user32")]
            public extern static bool ChangeWindowMessageFilterEx(IntPtr hwnd, int message, int action, int pChangeFilterStruct);

            [DllImport("user32")]
            public extern static bool ChangeWindowMessageFilter(IntPtr message, int action);

            /// Return Type: void
            ///param0: HWND->HWND__*
            ///param1: BOOL->int
            [System.Runtime.InteropServices.DllImportAttribute("shell32.dll", EntryPoint = "DragAcceptFiles", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
            public static extern void DragAcceptFiles(System.IntPtr param0, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool param1);

            /// Return Type: UINT->unsigned int
            ///param0: HDROP->HDROP__*
            ///param1: UINT->unsigned int
            ///param2: LPSTR->CHAR*
            ///param3: UINT->unsigned int
            [System.Runtime.InteropServices.DllImportAttribute("shell32.dll", EntryPoint = "DragQueryFile", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall, SetLastError = true)]
            public static extern uint DragQueryFile(System.IntPtr param0, uint param1, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] System.Text.StringBuilder param2, uint param3);

            /// Return Type: UINT->unsigned int
            ///param0: HDROP->HDROP__*
            ///param1: int-> int
            ///param2: LPSTR->CHAR*
            ///param3: UINT->unsigned int
            [System.Runtime.InteropServices.DllImportAttribute("shell32.dll", EntryPoint = "DragQueryFile", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
            public static extern uint DragQueryFile(System.IntPtr param0, int param1, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] System.Text.StringBuilder param2, uint param3);

            /// Return Type: void
            ///param0: HDROP->HDROP__*
            [System.Runtime.InteropServices.DllImportAttribute("shell32.dll", EntryPoint = "DragFinish", CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
            public static extern void DragFinish(System.IntPtr param0);

            /// Return Type: LONG->int
            ///hWnd: HWND->HWND__*
            ///nIndex: int
            ///dwNewLong: LONG->int
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetWindowLong")]
            public static extern IntPtr SetWindowLong([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int nIndex, IntPtr dwNewLong);

            /// Return Type: LRESULT->LONG_PTR->int
            ///lpPrevWndFunc: WNDPROC
            ///hWnd: HWND->HWND__*
            ///Msg: UINT->unsigned int
            ///wParam: WPARAM->UINT_PTR->unsigned int
            ///lParam: LPARAM->LONG_PTR->int
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "CallWindowProc")]
            public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, [System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        }

        #endregion
    }
}
