using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Golang.Ioc
{
    public class CStringMarshaler : ICustomMarshaler
    {
        private static readonly CStringMarshaler Instance = new CStringMarshaler();

        [ThreadStatic] private static IntPtr lastIntPtr;

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            if (lastIntPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(lastIntPtr);
                lastIntPtr = IntPtr.Zero;
            }
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            if (ReferenceEquals(managedObj, null))
            {
                return IntPtr.Zero;
            }

            if (!(managedObj is string))
            {
                throw new InvalidOperationException();
            }

            var utf8Bytes = Encoding.UTF8.GetBytes(managedObj as string);
            var ptr = Marshal.AllocHGlobal(utf8Bytes.Length + 1);
            Marshal.Copy(utf8Bytes, 0, ptr, utf8Bytes.Length);
            Marshal.WriteByte(ptr, utf8Bytes.Length, 0);
            return lastIntPtr = ptr;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
            {
                return null;
            }

            var bytes = new List<byte>();
            for (var offset = 0;; offset++)
            {
                var b = Marshal.ReadByte(pNativeData, offset);
                if (b == 0)
                {
                    break;
                }

                bytes.Add(b);
            }

            NativeLib.Free(pNativeData);
            var str = Encoding.UTF8.GetString(bytes.ToArray());
            return str;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }

      
    }
}