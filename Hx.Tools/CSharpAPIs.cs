using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Drawing;

namespace HX.Tools
{

    public static class CSharpWindowsAPI
    {
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);
        private delegate bool CHILDWNDENUMPROC(IntPtr hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);
        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, CHILDWNDENUMPROC lpfn, int lParam);
        //[DllImport("user32.dll")]
        //private static extern IntPtr FindWindowW(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll ", CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);
        /// <summary>
        /// 返回包含了指定点的窗口的句柄。忽略屏蔽、隐藏以及透明窗口
        /// </summary>
        /// <param name="Point">指定的鼠标坐标</param>
        /// <returns>鼠标坐标处的窗口句柄，如果没有，返回</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(Point Point);

        /// <summary>
        /// 获取鼠标处的坐标
        /// </summary>
        /// <param name="lpPoint">随同指针在屏幕像素坐标中的位置载入的一个结构</param>
        /// <returns>非零表示成功，零表示失败</returns>
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point lpPoint);

        private static int WM_SETTEXT = 0x000C;
        private static uint WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        private static uint WM_LBUTTONUP = 0x202;  //Left mousebutton up
        private static uint WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
        private static uint WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        private static uint WM_RBUTTONUP = 0x205;   //Right mousebutton up
        private static uint WM_RBUTTONDBLCLK = 0x206; //Right mousebutton doubleclick
        private static uint WM_KEYDOWN = 0x100;  //Key down
        private static uint WM_KEYUP = 0x101;   //Key up

        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
        }

        public static WindowInfo[] GetAllDesktopWindows()
        {
            List<WindowInfo> wndList = new List<WindowInfo>();

            //enum all desktop windows
            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);
                //get hwnd
                wnd.hWnd = hWnd;
                //get window name
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                //get window class
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                //add it into list
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList.ToArray();
        }

        public static WindowInfo[] GetChildWindows(IntPtr hWndParent)
        {
            List<WindowInfo> wndList = new List<WindowInfo>();

            //enum all desktop windows
            EnumChildWindows(hWndParent, delegate(IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);
                //get hwnd
                wnd.hWnd = hWnd;
                //get window name
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                //get window class
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                //add it into list
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList.ToArray();
        }

        public static void SendText(IntPtr hWnd, string text)
        {
            SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, text);
        }

        public static void LeftMouseDown(IntPtr hWnd)
        {
            int x = 10; // X coordinate of the click 
            int y = 10; // Y coordinate of the click 
            IntPtr lParam = (IntPtr)((y << 16) | x); // The coordinates 
            IntPtr wParam = IntPtr.Zero; // Additional parameters for the click (e.g. Ctrl) 
            SendMessage(hWnd, WM_LBUTTONDOWN, wParam, lParam);
            SendMessage(hWnd, WM_LBUTTONUP, wParam, lParam); 
        }
    }

}
