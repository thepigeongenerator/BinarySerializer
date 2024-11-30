#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace ThePigeonGenerator.Util
{
    public static class BinarySerializer
    {
        // utility method for deserializing from a buffer pointer
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe T? Deserialize<T>(byte* pBuf)
        {
            return Marshal.PtrToStructure<T>((IntPtr)pBuf);
        }

        // utility method for serializing into a buffer pointer
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void Serialize<T>(T obj, byte* pBuf) where T : struct
        {
            Marshal.StructureToPtr<T>(obj, (IntPtr)pBuf, false);
        }

        // deserializes T from the specified buffer
        public static unsafe T? Deserialize<T>(byte[] buf) where T : struct
        {
            // I am assuming that the programmer is not stupid and will read from the correct type of buffer, hence me not checking the sizes
            T? obj;

            // get the array as a pointer
            fixed (byte* pBuf = &buf[0])
            {
                // store the data of the pointer as the desired object
                obj = Deserialize<T>(pBuf);
            }

            return obj;
        }

        // serializes the object into it's binary representation to the buffer
        public static unsafe byte[] Serialize<T>(T obj) where T : struct
        {
            // allocate a buffer with the size of T
            byte[] buf = new byte[Marshal.SizeOf<T>()];

            // get the array as a pointer (all arrays are a pointer)
            fixed (byte* pBuf = &buf[0])
            {
                // store the structure to the pointer which points to the buffer
                Serialize<T>(obj, pBuf);
            }

            return buf;
        }

        //
        // array serialization / deserialization
        //

        // deserializes an array of T from the specified buffer
        public static unsafe T?[] DeserializeArray<T>(byte[] buf)
        {
            int size = Marshal.SizeOf<T>();         // get how much memory T occupies
            T?[] arr = new T?[buf.Length / size];   // allocate an array of T with the buffer's size divided by T's size to get the original T array size

            for (int i = 0; i < arr.Length; i++)
            {
                // get a pointer using the input array's index to map everything correctly
                fixed (byte* pBuf = &buf[i * size])
                {
                    arr[i] = Deserialize<T>(pBuf);
                }
            }

            return arr;
        }

        // serializes an array of T to it's binary representation to the buffer
        public static unsafe byte[] SerializeArray<T>(params T[] arr) where T : struct
        {
            // allocate a buffer with the size of T * the array's length
            int size = Marshal.SizeOf<T>();             // get how much memory T occupies
            byte[] buf = new byte[size * arr.Length];   // allocate a byte array the size of the T array

            for (int i = 0; i < arr.Length; i++)
            {
                // get a pointer using the input array's index to map everything correctly
                fixed (byte* pBuf = &buf[i * size])
                {
                    // store the structure to the pointer in the buffer
                    Serialize<T>(arr[i], pBuf);
                }
            }

            return buf;
        }
    }
}
