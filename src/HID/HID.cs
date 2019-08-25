using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



public abstract class HIDInput {
    public HIDInput() {    }
    public long Time { get; set; }
}


public static class HID {
    //public static event EventHandler MouseAction = delegate { };
    private const int WH_MOUSE_LL = 14;
    private const int WH_KEYBOARD_LL = 13;
    private static Stopwatch stopWatch = new Stopwatch();
    //mouse
    public delegate void MouseStatusHandler(object sender, MouseEventArgs e);
    public static event MouseStatusHandler MouseAction = delegate { };
    //keyboard
    public delegate void KeyboardStatusHandler(object sender, KeyboardEventArgs e);
    public static event KeyboardStatusHandler KeyboardAction = delegate { };

    public static void Start() {
        _hookID_Mouse = SetHook(WH_MOUSE_LL, _proc_mouse);
        _hookID_Keyboard = SetHook(WH_KEYBOARD_LL, _proc_keyboard);
        stopWatch.Start();

    }
    public static void stop() {
        UnhookWindowsHookEx(_hookID_Mouse);
        UnhookWindowsHookEx(_hookID_Keyboard);
        stopWatch.Stop();
    }

    private static LowLevelProc _proc_mouse = HookCallback_Mouse;
    private static LowLevelProc _proc_keyboard = HookCallback_Keyboard;

    private static IntPtr _hookID_Mouse = IntPtr.Zero;
    private static IntPtr _hookID_Keyboard = IntPtr.Zero;

    private static IntPtr SetHook(int idHook, LowLevelProc proc) {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule) {
            return SetWindowsHookEx(idHook, proc,
              GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback_Mouse(int nCode, IntPtr wParam, IntPtr lParam) {
        if (nCode >= 0 && (
            (MouseStatus)wParam == MouseStatus.WM_LBUTTONDOWN ||
            (MouseStatus)wParam == MouseStatus.WM_LBUTTONUP ||
            (MouseStatus)wParam == MouseStatus.WM_RBUTTONDOWN ||
            (MouseStatus)wParam == MouseStatus.WM_RBUTTONUP)) {
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            Point mouse = System.Windows.Forms.Cursor.Position;
            MouseAction(null, new MouseEventArgs( new HIDMouseInput( (MouseStatus)wParam, mouse.X, mouse.Y, stopWatch.ElapsedMilliseconds)));
        }
        return CallNextHookEx(_hookID_Mouse, nCode, wParam, lParam);
    }

    private static IntPtr HookCallback_Keyboard(int nCode, IntPtr wParam, IntPtr lParam) {
        if (nCode >= 0) {
            int vkCode = Marshal.ReadInt32(lParam);
            Console.WriteLine((Keys)vkCode);
            KeyboardAction(null, new KeyboardEventArgs( new HIDKeyInput( (Keys)vkCode, (KeyStatus)wParam, stopWatch.ElapsedMilliseconds)));
        }
        return CallNextHookEx(_hookID_Keyboard, nCode, wParam, lParam);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
      LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
      IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);



}
