using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BisoProject
{
    class KeyBoardInputHooking
    {
        [DllImport("User32.dll")]
        static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

        public static void OnString(string key)
        {
            for (int i = 0; i < key.Length; i++)
            {
                int ascii;
                if (97 <= Convert.ToInt32(Convert.ToChar(key.Substring(i, 1))) && Convert.ToInt32(Convert.ToChar(key.Substring(i, 1))) <= 122)
                    ascii = Convert.ToInt32(Convert.ToChar(key.Substring(i, 1))) - 32;
                else
                    ascii = Convert.ToInt32(Convert.ToChar(key.Substring(i, 1)));

                keybd_event((byte)ascii, 0x00, 0x00, 0);
                Thread.Sleep(100);
                keybd_event((byte)ascii, 0x00, 0x02, 0);
                Thread.Sleep(100);
            }
        }

        public static void SpecKey(string key)
        {
            switch (key)
            {
                case "Enter":
                    keybd_event((byte)13, 0x00, 0x00, 0);
                    Thread.Sleep(100);
                    keybd_event((byte)13, 0x00, 0x02, 0);
                    Thread.Sleep(1000);
                    break;

                case "Tab":
                    keybd_event((byte)9, 0x00, 0x00, 0);
                    Thread.Sleep(100);
                    keybd_event((byte)9, 0x00, 0x02, 0);
                    break;

                default:
                    break;
            }
        }
    }
}
