using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SensitivityMatcherXAML
{
    public static class Native
    {
        public static uint MOUSEEVENTF_ABSOLUTE = 0x80000;
        public static uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        public static uint MOUSEEVENTF_LEFTUP = 0x0004;
        public static uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public static uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        public static uint MOUSEEVENTF_MOVE = 0x0001;
        public static uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public static uint MOUSEEVENTF_RIGHTUP = 0x0010;
        public static uint MOUSEEVENTF_XDOWN = 0x0080;
        public static uint MOUSEEVENTF_XUP = 0x0100;
        public static uint MOUSEEVENTF_WHEEL = 0x0800;
        public static uint MOUSEEVENTF_HWHEEL = 0x01000;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("User32.dll")]
        public static extern bool RegisterHotKey([In] IntPtr hWnd, [In] int id, [In] uint fsModifiers, [In] uint vk);

        [DllImport("User32.dll")]
        public static extern bool UnregisterHotKey([In] IntPtr hWnd, [In] int id);
    }
}
