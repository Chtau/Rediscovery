using Rediscovery.Feature.Shared.Functions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Shared.Functions
{
    public static class RemoteProcessControl
    {
        public static void SendKeys(IntPtr windowHandle, KeyCode[] keyCodes, 
            bool altKeyPressed = false, bool controlKeyPressed = false, bool shiftKeyPressed = false)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            if (altKeyPressed)
                WindowSendInput.SendKeyDown(KeyCode.ALT);
            if (controlKeyPressed)
                WindowSendInput.SendKeyDown(KeyCode.CONTROL);
            if (shiftKeyPressed)
                WindowSendInput.SendKeyDown(KeyCode.SHIFT);
            foreach (var key in keyCodes)
            {
                WindowSendInput.SendKeyPress(key);
            }
            if (altKeyPressed)
                WindowSendInput.SendKeyUp(KeyCode.ALT);
            if (controlKeyPressed)
                WindowSendInput.SendKeyUp(KeyCode.CONTROL);
            if (shiftKeyPressed)
                WindowSendInput.SendKeyUp(KeyCode.SHIFT);
        }

        public static void SendKeyDown(IntPtr windowHandle, KeyCode key)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            WindowSendInput.SendKeyDown(key);
        }

        public static void SendKeyUp(IntPtr windowHandle, KeyCode key)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            WindowSendInput.SendKeyUp(key);
        }

        public static void SendKeyPress(IntPtr windowHandle, KeyCode key)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            WindowSendInput.SendKeyPress(key);
        }
    }
}
