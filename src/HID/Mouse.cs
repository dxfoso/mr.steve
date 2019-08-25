using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

 

    public enum MouseStatus {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }
 
    public class MouseEventArgs : EventArgs {
        public MouseEventArgs(MouseStatus mouseStatus, int xPos, int yPos, long time) {
            MouseStatus = mouseStatus;
            MousePosX = xPos;
            MousePosY = yPos;
            Time = time;
        }
        public MouseStatus MouseStatus { get; set; }
        public int MousePosX { get; set; }
        public int MousePosY { get; set; }
        public long Time { get; set; }
    }
 