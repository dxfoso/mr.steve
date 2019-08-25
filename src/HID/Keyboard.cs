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
        public KeyboardEventArgs(Keys key, KeyStatus status, long time) {
            Key = key;
            Status = status;
            Time = time;
        }
        public Keys Key { get; set; }
        public KeyStatus Status { get; set; }
        public long Time { get; set; }
    }
 