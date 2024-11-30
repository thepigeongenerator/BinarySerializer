#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace ThePigeonGenerator.Util
{
    public static class BinarySerializer
    {
        // deserializes T from the specified buffer
        [Obsolete("under development")]
        public static unsafe T? Deserialize<T>(byte[] buf) where T : struct
        {
            // I am assuming that the programmer is not stupid and will read from the correct type of buffer, hence me not checking the sizes
            T? obj;

            // get the array as a pointer
            fixed (byte* pBuf = &buf[0])
            {
                // store the data of the pointer as the desired object
                obj = Marshal.PtrToStructure<T>((IntPtr)pBuf);
            }

            return obj;
        }

        // serializes the object into it's binary representation to the buffer
        public static unsafe byte[] Serialize<T>(T obj) where T : struct
        {
            // allocate a buffer with the size of T
            Type t = typeof(T);
            int size = ReflectionUtil.GetStructureSize(t, obj);
            byte[] buf = new byte[size];

            // get the array as a pointer (all arrays are a pointer)
            fixed (byte* pBuf = &buf[0])
            {
                // store the structure to the pointer which points to the buffer
                ReflectionUtil.SerializeStructure(t, obj, pBuf, size);
            }

            return buf;
        }
    }
}
