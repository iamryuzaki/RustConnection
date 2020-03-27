using System;
using System.Runtime.InteropServices;

namespace RustConnection.Steamwork
{
    [StructLayout(LayoutKind.Sequential, Size = 0xea, Pack = 1)]
    public struct Token
    {
        public uint Length;
        public ulong ID;
        public ulong SteamID;
        public uint ConnectionTime;
        public Session Session;
        public TokenInformation Information;

        public static Token Parse(byte[] buffer)
        {
            try
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                Token local = (Token)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Token));
                handle.Free();
                return local;
            }
            catch
            {
            }
            return default(Token);
        }
        
        public static byte[] getBytes(Token str) {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }
}
