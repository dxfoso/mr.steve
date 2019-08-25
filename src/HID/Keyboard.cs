using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



public enum KeyStatus {
    WM_KEYDOWN = 0x0100,
    WM_KEYUP = 0x0101
}

public class KeyboardEventArgs : EventArgs {

    public KeyboardEventArgs() { }

    public KeyboardEventArgs(HIDKeyInput input ) {
        KeyInput = input;
    }
    public HIDKeyInput KeyInput  { get; set; }
  
}



public class HIDKeyInput : HIDInput {
    public HIDKeyInput() { }

    public HIDKeyInput(Keys key, KeyStatus status, int time) {
        Key = key;
        Status = status;
        Time = time;
    }
    public Keys Key { get; set; }
    public KeyStatus Status { get; set; }

    public override void _play() {
        int keystatus=0;
        keystatus = Status == KeyStatus.WM_KEYDOWN ? 0 : keystatus;
        keystatus = Status == KeyStatus.WM_KEYUP ? 2 : keystatus;

        keybd_event((byte)(int)Key, (byte)MapVirtualKey((int)Key, 0), keystatus, 0); // V Down
    }
     
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int extraInfo);

    [DllImport("user32.dll")]
    static extern short MapVirtualKey(int wCode, int wMapType);

     



}



