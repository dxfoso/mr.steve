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
        public MouseEventArgs(HIDMouseInput input ) {
        MouseInput = input;
        }
        public HIDMouseInput MouseInput { get; set; }
 
    }


public class HIDMouseInput : HIDInput {

    public HIDMouseInput() { }

    public HIDMouseInput(MouseStatus mouseStatus, int xPos, int yPos, int time){
        Status = mouseStatus;
        PosX = xPos;
        PosY = yPos;
        Time = time;

    }

    public MouseStatus Status { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    public override void _play() {
         

        Cursor.Position = new System.Drawing.Point(PosX , PosY);

        switch (Status) {
            case MouseStatus.WM_LBUTTONDOWN:
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, new System.IntPtr());
                break;
            case MouseStatus.WM_LBUTTONUP:
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, new System.IntPtr());
                break;


            case MouseStatus.WM_RBUTTONDOWN:
                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, new System.IntPtr());
                break;
            case MouseStatus.WM_RBUTTONUP:
                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, new System.IntPtr());
                break;
                
        }
       

    }

    private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
    private const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const UInt32 MOUSEEVENTF_RIGHTUP = 0x0010;
    
    [DllImport("user32.dll")]
    private static extern void mouse_event(
           UInt32 dwFlags, // motion and click options
           UInt32 dx, // horizontal position or change
           UInt32 dy, // vertical position or change
           UInt32 dwData, // wheel movement
           IntPtr dwExtraInfo // application-defined information
    );
}
