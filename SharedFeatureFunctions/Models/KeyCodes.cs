using System;
using System.Collections.Generic;
using System.Text;

namespace Rediscovery.Feature.Shared.Functions.Models
{
    public enum KeyCode : ushort
    {
        #region Media

        /// <summary>
        /// Next track if a song is playing
        /// </summary>
        MEDIA_NEXT_TRACK = 0xb0,

        /// <summary>
        /// Play pause
        /// </summary>
        MEDIA_PLAY_PAUSE = 0xb3,

        /// <summary>
        /// Previous track
        /// </summary>
        MEDIA_PREV_TRACK = 0xb1,

        /// <summary>
        /// Stop
        /// </summary>
        MEDIA_STOP = 0xb2,

        #endregion

        #region math

        /// <summary>Key "+"</summary>
        ADD = 0x6b,
        /// <summary>
        /// "*" key
        /// </summary>
        MULTIPLY = 0x6a,

        /// <summary>
        /// "/" key
        /// </summary>
        DIVIDE = 0x6f,

        /// <summary>
        /// Subtract key "-"
        /// </summary>
        SUBTRACT = 0x6d,

        #endregion

        #region Browser
        /// <summary>
        /// Go Back
        /// </summary>
        BROWSER_BACK = 0xa6,
        /// <summary>
        /// Favorites
        /// </summary>
        BROWSER_FAVORITES = 0xab,
        /// <summary>
        /// Forward
        /// </summary>
        BROWSER_FORWARD = 0xa7,
        /// <summary>
        /// Home
        /// </summary>
        BROWSER_HOME = 0xac,
        /// <summary>
        /// Refresh
        /// </summary>
        BROWSER_REFRESH = 0xa8,
        /// <summary>
        /// browser search
        /// </summary>
        BROWSER_SEARCH = 170,
        /// <summary>
        /// Stop
        /// </summary>
        BROWSER_STOP = 0xa9,
        #endregion

        #region Numpad numbers
        /// <summary>
        /// NUMPAD 0
        /// </summary>
        NUMPAD0 = 0x60,
        /// <summary>
        /// NUMPAD 1
        /// </summary>
        NUMPAD1 = 0x61,
        /// <summary>
        /// NUMPAD 2
        /// </summary>
        NUMPAD2 = 0x62,
        /// <summary>
        /// NUMPAD 3
        /// </summary>
        NUMPAD3 = 0x63,
        /// <summary>
        /// NUMPAD 4
        /// </summary>
        NUMPAD4 = 100,
        /// <summary>
        /// NUMPAD 5
        /// </summary>
        NUMPAD5 = 0x65,
        /// <summary>
        /// NUMPAD 6
        /// </summary>
        NUMPAD6 = 0x66,
        /// <summary>
        /// NUMPAD 7
        /// </summary>
        NUMPAD7 = 0x67,
        /// <summary>
        /// NUMPAD 8
        /// </summary>
        NUMPAD8 = 0x68,
        /// <summary>
        /// NUMPAD 9
        /// </summary>
        NUMPAD9 = 0x69,

        #endregion

        #region Fkeys
        /// <summary>
        /// F1
        /// </summary>
        F1 = 0x70,
        /// <summary>
        /// F10
        /// </summary>
        F10 = 0x79,
        /// <summary>
        /// F11
        /// </summary>
        F11 = 0x7a,
        /// <summary>
        /// F12
        /// </summary>
        F12 = 0x7b,
        /// <summary>
        /// F13
        /// </summary>
        F13 = 0x7c,
        /// <summary>
        /// F14
        /// </summary>
        F14 = 0x7d,
        /// <summary>
        /// F15
        /// </summary>
        F15 = 0x7e,
        /// <summary>
        /// F16
        /// </summary>
        F16 = 0x7f,
        /// <summary>
        /// F17
        /// </summary>
        F17 = 0x80,
        /// <summary>
        /// F18
        /// </summary>
        F18 = 0x81,
        /// <summary>
        /// F19
        /// </summary>
        F19 = 130,
        /// <summary>
        /// F2
        /// </summary>
        F2 = 0x71,
        /// <summary>
        /// F20
        /// </summary>
        F20 = 0x83,
        /// <summary>
        /// F21
        /// </summary>
        F21 = 0x84,
        /// <summary>
        /// F22
        /// </summary>
        F22 = 0x85,
        /// <summary>
        /// F23
        /// </summary>
        F23 = 0x86,
        /// <summary>
        /// F24
        /// </summary>
        F24 = 0x87,
        /// <summary>
        /// F3
        /// </summary>
        F3 = 0x72,
        /// <summary>
        /// F4
        /// </summary>
        F4 = 0x73,
        /// <summary>
        /// F5
        /// </summary>
        F5 = 0x74,
        /// <summary>
        /// F6
        /// </summary>
        F6 = 0x75,
        /// <summary>
        /// F7
        /// </summary>
        F7 = 0x76,
        /// <summary>
        /// F8
        /// </summary>
        F8 = 0x77,
        /// <summary>
        /// F9
        /// </summary>
        F9 = 120,

        #endregion

        #region Other
        /// <summary>
        /// OEM 1
        /// </summary>
        OEM_1 = 0xba,
        /// <summary>
        /// OEM 102
        /// </summary>
        OEM_102 = 0xe2,
        /// <summary>
        /// OEM 2
        /// </summary>
        OEM_2 = 0xbf,
        /// <summary>
        /// OEM 3
        /// </summary>
        OEM_3 = 0xc0,
        /// <summary>
        /// OEM 4
        /// </summary>
        OEM_4 = 0xdb,
        /// <summary>
        /// OEM 5
        /// </summary>
        OEM_5 = 220,
        /// <summary>
        /// OEM 6
        /// </summary>
        OEM_6 = 0xdd,
        /// <summary>
        /// OEM 7
        /// </summary>
        OEM_7 = 0xde,
        /// <summary>
        /// OEM 8
        /// </summary>
        OEM_8 = 0xdf,
        /// <summary>
        /// OEM CLEAR
        /// </summary>
        OEM_CLEAR = 0xfe,
        /// <summary>
        /// OEM COMMA
        /// </summary>
        OEM_COMMA = 0xbc,
        /// <summary>
        /// OEM MINUS
        /// </summary>
        OEM_MINUS = 0xbd,
        /// <summary>
        /// OEM PERIOD
        /// </summary>
        OEM_PERIOD = 190,
        /// <summary>
        /// OEM PLUS
        /// </summary>
        OEM_PLUS = 0xbb,

        #endregion

        #region KEYS

        /// <summary>
        /// 0
        /// </summary>
        KEY_0 = 0x30,
        /// <summary>
        /// 1
        /// </summary>
        KEY_1 = 0x31,
        /// <summary>
        /// 2
        /// </summary>
        KEY_2 = 50,
        /// <summary>
        /// 3
        /// </summary>
        KEY_3 = 0x33,
        /// <summary>
        /// 4
        /// </summary>
        KEY_4 = 0x34,
        /// <summary>
        /// 5
        /// </summary>
        KEY_5 = 0x35,
        /// <summary>
        /// 6
        /// </summary>
        KEY_6 = 0x36,
        /// <summary>
        /// 7
        /// </summary>
        KEY_7 = 0x37,
        /// <summary>
        /// 8
        /// </summary>
        KEY_8 = 0x38,
        /// <summary>
        /// 9
        /// </summary>
        KEY_9 = 0x39,
        /// <summary>
        /// A
        /// </summary>
        KEY_A = 0x41,
        /// <summary>
        /// B
        /// </summary>
        KEY_B = 0x42,
        /// <summary>
        /// C
        /// </summary>
        KEY_C = 0x43,
        /// <summary>
        /// D
        /// </summary>
        KEY_D = 0x44,
        /// <summary>
        /// E
        /// </summary>
        KEY_E = 0x45,
        /// <summary>
        /// F
        /// </summary>
        KEY_F = 70,
        /// <summary>
        /// G
        /// </summary>
        KEY_G = 0x47,
        /// <summary>
        /// H
        /// </summary>
        KEY_H = 0x48,
        /// <summary>
        /// I
        /// </summary>
        KEY_I = 0x49,
        /// <summary>
        /// J
        /// </summary>
        KEY_J = 0x4a,
        /// <summary>
        /// K
        /// </summary>
        KEY_K = 0x4b,
        /// <summary>
        /// L
        /// </summary>
        KEY_L = 0x4c,
        /// <summary>
        /// M
        /// </summary>
        KEY_M = 0x4d,
        /// <summary>
        /// N
        /// </summary>
        KEY_N = 0x4e,
        /// <summary>
        /// O
        /// </summary>
        KEY_O = 0x4f,
        /// <summary>
        /// P
        /// </summary>
        KEY_P = 80,
        /// <summary>
        /// Q
        /// </summary>
        KEY_Q = 0x51,
        /// <summary>
        /// R
        /// </summary>
        KEY_R = 0x52,
        /// <summary>
        /// S
        /// </summary>
        KEY_S = 0x53,
        /// <summary>
        /// T
        /// </summary>
        KEY_T = 0x54,
        /// <summary>
        /// U
        /// </summary>
        KEY_U = 0x55,
        /// <summary>
        /// V
        /// </summary>
        KEY_V = 0x56,
        /// <summary>
        /// W
        /// </summary>
        KEY_W = 0x57,
        /// <summary>
        /// X
        /// </summary>
        KEY_X = 0x58,
        /// <summary>
        /// Y
        /// </summary>
        KEY_Y = 0x59,
        /// <summary>
        /// Z
        /// </summary>
        KEY_Z = 90,

        #endregion

        #region volume
        /// <summary>
        /// Decrese volume
        /// </summary>
        VOLUME_DOWN = 0xae,

        /// <summary>
        /// Mute volume
        /// </summary>
        VOLUME_MUTE = 0xad,

        /// <summary>
        /// Increase volue
        /// </summary>
        VOLUME_UP = 0xaf,

        #endregion


        /// <summary>
        /// Take snapshot of the screen and place it on the clipboard
        /// </summary>
        SNAPSHOT = 0x2c,

        /// <summary>Send right click from keyboard "key that is 2 keys to the right of space bar"</summary>
        RightClick = 0x5d,

        /// <summary>
        /// Go Back or delete
        /// </summary>
        BACKSPACE = 8,

        /// <summary>
        /// Control + Break "When debuging if you step into an infinite loop this will stop debug"
        /// </summary>
        CANCEL = 3,
        /// <summary>
        /// Caps lock key to send cappital letters
        /// </summary>
        CAPS_LOCK = 20,
        /// <summary>
        /// Ctlr key
        /// </summary>
        CONTROL = 0x11,

        /// <summary>
        /// Alt key
        /// </summary>
        ALT = 18,

        /// <summary>
        /// "." key
        /// </summary>
        DECIMAL = 110,

        /// <summary>
        /// Delete Key
        /// </summary>
        DELETE = 0x2e,


        /// <summary>
        /// Arrow down key
        /// </summary>
        DOWN = 40,

        /// <summary>
        /// End key
        /// </summary>
        END = 0x23,

        /// <summary>
        /// Escape key
        /// </summary>
        ESC = 0x1b,

        /// <summary>
        /// Home key
        /// </summary>
        HOME = 0x24,

        /// <summary>
        /// Insert key
        /// </summary>
        INSERT = 0x2d,

        /// <summary>
        /// Open my computer
        /// </summary>
        LAUNCH_APP1 = 0xb6,
        /// <summary>
        /// Open calculator
        /// </summary>
        LAUNCH_APP2 = 0xb7,

        /// <summary>
        /// Open default email in my case outlook
        /// </summary>
        LAUNCH_MAIL = 180,

        /// <summary>
        /// Opend default media player (itunes, winmediaplayer, etc)
        /// </summary>
        LAUNCH_MEDIA_SELECT = 0xb5,

        /// <summary>
        /// Left control
        /// </summary>
        LCONTROL = 0xa2,

        /// <summary>
        /// Left arrow
        /// </summary>
        LEFT = 0x25,

        /// <summary>
        /// Left shift
        /// </summary>
        LSHIFT = 160,

        /// <summary>
        /// left windows key
        /// </summary>
        LWIN = 0x5b,


        /// <summary>
        /// Next "page down"
        /// </summary>
        PAGEDOWN = 0x22,

        /// <summary>
        /// Num lock to enable typing numbers
        /// </summary>
        NUMLOCK = 0x90,

        /// <summary>
        /// Page up key
        /// </summary>
        PAGE_UP = 0x21,

        /// <summary>
        /// Right control
        /// </summary>
        RCONTROL = 0xa3,

        /// <summary>
        /// Return key
        /// </summary>
        ENTER = 13,

        /// <summary>
        /// Right arrow key
        /// </summary>
        RIGHT = 0x27,

        /// <summary>
        /// Right shift
        /// </summary>
        RSHIFT = 0xa1,

        /// <summary>
        /// Right windows key
        /// </summary>
        RWIN = 0x5c,

        /// <summary>
        /// Shift key
        /// </summary>
        SHIFT = 0x10,

        /// <summary>
        /// Space back key
        /// </summary>
        SPACE_BAR = 0x20,

        /// <summary>
        /// Tab key
        /// </summary>
        TAB = 9,

        /// <summary>
        /// Up arrow key
        /// </summary>
        UP = 0x26,

    }
}
