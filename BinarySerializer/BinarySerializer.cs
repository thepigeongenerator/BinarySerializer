#nullable enable
using System;
using uint8 = System.Byte;
using int32 = System.Int32;

namespace ThePigeonGenerator.Util
{
    public static class BinarySerializer
    {
        // serializes the object into it's binary representation to the buffer
        public static unsafe uint8[] Serialize<T>(T obj) => Serialize(typeof(T), obj ?? throw new NullReferenceException("inputted object cannot be null!"));
        public static unsafe uint8[] Serialize(Type t, object obj)
        {
            if (obj == null)
                throw new NullReferenceException("the object is not allowed to be null!");

            // allocate a buffer with the size of T
            int32 size = ReflectionUtil.GetStructureSize(t, obj);
            uint8[] buf = new byte[size];

            // get the array as a pointer (all arrays are a pointer)
            fixed (uint8* pBuf = &buf[0])
            {
                // store the structure to the pointer which points to the buffer
                ReflectionUtil.SerializeStructure(t, obj, pBuf, size);
            }

            return buf;
        }

        // deserializes T from the specified buffer
        public static unsafe T? Deserialize<T>(uint8[] buf) => (T?)Deserialize(typeof(T), buf);
        public static unsafe object? Deserialize(Type t, uint8[] buf)
        {
            // I am assuming that the programmer is not stupid and will read from the correct type of buffer, hence me not checking the sizes
            object? obj;

            // get the array as a pointer
            fixed (uint8* pBuf = &buf[0])
            {
                // store the data of the pointer as the desired object
                obj = ReflectionUtil.DeserializeStructure(t, pBuf);
            }

            return obj;
        }
    }
}
