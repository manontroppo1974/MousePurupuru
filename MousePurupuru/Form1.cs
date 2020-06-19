using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MousePurupuru
{
    public partial class Form1 : Form
    {
        TimerCallback timerDelegate = null;
        System.Threading.Timer timer = null;
        const int timerPeriod = 30000;
        const int timerDue = 30000;

        public Form1()
        {
            InitializeComponent();

            timerDelegate = new TimerCallback(timer_Tick);
            timer = new System.Threading.Timer(timerDelegate, null, System.Threading.Timeout.Infinite, timerPeriod);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                timer.Change(timerDue, timerPeriod);
            }
            else
            {
                timer.Change(System.Threading.Timeout.Infinite, timerPeriod);
            }
        }

        public void timer_Tick(object sender)
        {
            PreventScreenSaverFromStarting();
        }

        void PreventScreenSaverFromStarting()
        {
            INPUT input = new INPUT();
            input.type = INPUT_MOUSE;
            input.mi = new MOUSEINPUT();

            input.mi.dwExtraInfo = IntPtr.Zero;
            input.mi.dx = 0;
            input.mi.dy = 0;
            input.mi.time = 0;
            input.mi.mouseData = 0;
            input.mi.dwFlags = 0x0001; // MOVE (RELATIVE)
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            uint r = SendInput(1, ref input, cbSize);
        }

        #region Win32 API
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        const int INPUT_MOUSE = 0;

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            ushort wVk;
            ushort wScan;
            uint dwFlags;
            uint time;
            IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            uint uMsg;
            ushort wParamL;
            ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)] //*
            public MOUSEINPUT mi;
            [FieldOffset(4)] //*
            public KEYBDINPUT ki;
            [FieldOffset(4)] //*
            public HARDWAREINPUT hi;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            // Legacy flag, should not be used.
            // ES_USER_PRESENT   = 0x00000004,
            ES_CONTINUOUS = 0x80000000,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        #endregion
    }
}
