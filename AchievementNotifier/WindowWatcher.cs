using AchievementNotifier.Parsers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AchievementNotifier
{
    internal class WindowWatcher
    {
        private WinEventDelegate dele = null;
        private EmuDetector emuDetector;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        public WindowWatcher()
        {
            emuDetector = new EmuDetector();
            dele = new WinEventDelegate(WinEventProc);
            IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            uint pid = GetActiveWindowProcessId(hwnd);
            emuDetector.checkProcessForEmu((int)pid);
        }

        private uint GetActiveWindowProcessId(IntPtr hwnd)
        {
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            if (pid > 0)
            {
                return pid;
            }
            return 0;
        }


        
    }
}
