using System;
using System.Collections.Generic;
using System.Text;

namespace SharedFeatureFunctions
{
    public static class RemoteProcessControl
    {
        public static void SendKeys(IntPtr windowHandle, RemoteProcessKeyCodes.KeyCode[] keyCodes, 
            bool altKeyPressed = false, bool controlKeyPressed = false, bool shiftKeyPressed = false)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            if (altKeyPressed)
                WindowSendInput.SendKeyDown(RemoteProcessKeyCodes.KeyCode.ALT);
            if (controlKeyPressed)
                WindowSendInput.SendKeyDown(RemoteProcessKeyCodes.KeyCode.CONTROL);
            if (controlKeyPressed)
                WindowSendInput.SendKeyDown(RemoteProcessKeyCodes.KeyCode.SHIFT);
            foreach (var key in keyCodes)
            {
                WindowSendInput.SendKeyPress(key);
            }
            if (altKeyPressed)
                WindowSendInput.SendKeyUp(RemoteProcessKeyCodes.KeyCode.ALT);
            if (controlKeyPressed)
                WindowSendInput.SendKeyUp(RemoteProcessKeyCodes.KeyCode.CONTROL);
            if (controlKeyPressed)
                WindowSendInput.SendKeyUp(RemoteProcessKeyCodes.KeyCode.SHIFT);
        }

        public static void SendKeyDown(IntPtr windowHandle, RemoteProcessKeyCodes.KeyCode key)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            WindowSendInput.SendKeyDown(key);
        }

        public static void SendKeyUp(IntPtr windowHandle, RemoteProcessKeyCodes.KeyCode key)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            WindowSendInput.SendKeyUp(key);
        }

        public static void SendKeyPress(IntPtr windowHandle, RemoteProcessKeyCodes.KeyCode key)
        {
            Native.Windows.Native.SetForegroundWindow(windowHandle);
            WindowSendInput.SendKeyPress(key);
        }
    }
}
