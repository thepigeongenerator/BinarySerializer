#nullable enable
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ThePigeonGenerator.Util;

internal static class ReflectionUtil
{
    private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    #region size utility
    // gets the size of an array datastructure
    private static int GetArraySize(Type t, object obj)
    {
        // get the element type and check if it's valid
        Type elmtType = t.GetElementType() ?? throw new NullReferenceException($"failed to get the element type of array {t.FullName}");
        if (elmtType.IsArray) throw new NotSupportedException($"an error occurred when processing '{t.FullName}' => we do not support multi-dimensional array of this nature, due to their dynamic and thus unknown size");

        int arrSize = GetStructureSize(elmtType, null); // get the size of the structure
        int arrLen = ((Array)obj).Length;               // get the length of the array (note: in the case of a multi-dimensional array, this'll contain the amount of items across all dimensions)
        return arrSize * arrLen;                        // multiply together to get the total size
    }

    // gets the size of a data structure, also serves as a means to validate that the data is in the correct format
    public static int GetStructureSize(Type t, object? obj)
    {
        // if t is an array, get the size of the array
        if (t.IsArray)
            return GetArraySize(t, obj ?? throw new NullReferenceException($"tried to get the size of array '{t.FullName}', but the specified array was 'null'"));

        // if t isn't a value type, we don't support it
        if (t.IsValueType == false)
            throw new NotSupportedException($"type {t.FullName} is not a value type and thus unsupported, due to it's unknown size");

        // if t is a primitive type (int, float, bool), we can get the size
        if (t.IsPrimitive)
            return Marshal.SizeOf(t);

        // otherwise, loop through the defined fields and call ourselves on these types
        // sum this result
        return (
            from f in t.GetFields(BINDING_FLAGS)
            select GetStructureSize(f.FieldType, obj == null ? null : f.GetValue(obj))
        ).Sum();
    }
    #endregion // size utility

    #region serialization
    private static unsafe void SerializeArray(Type t, object obj, byte* buf, int size)
    {
        Type elmtType = t.GetElementType() ?? throw new NullReferenceException($"failed to get the element type of '{t.FullName}'!");
        Array arr = (Array)obj;

        if (size % arr.Length != 0)
            throw new Exception("the array's size was not devisable by the element count");

        // get the element size by deviding the array size by the length
        int elmtSize = size / arr.Length;

        for (int i = 0; i < arr.Length; i++)
        {
            object elmt = arr.GetValue(i) ?? throw new NullReferenceException($"failed to get an element with the index of '{i}' in array of type '{t.FullName}'");
            SerializeStructure(elmtType, elmt, &buf[elmtSize * i], elmtSize);
        }
    }

    public static unsafe void SerializeStructure(Type t, object obj, byte* buf, int size)
    {
        // if T is a primitive type (int, float, bool), we can just convert it straight away
        if (t.IsPrimitive)
        {
            Marshal.StructureToPtr(obj, (IntPtr)buf, false);
            return;
        }

        // if T is an array, we can just serialize the array
        if (t.IsArray)
        {
            SerializeArray(t, obj, buf, size);
            return;
        }

        // loop through each field in t, and call ourselves
        int i = 0;
        foreach (FieldInfo f in t.GetFields(BINDING_FLAGS))
        {
            // get the structure size of this field
            object val = f.GetValue(obj) ?? throw new NullReferenceException($"field '{f.FieldType.FullName}' in '{t.FullName}' is not allowed to be null!");
            int s = GetStructureSize(f.FieldType, val);
            SerializeStructure(f.FieldType, val, &buf[i], s);
            i += s;
        }
    }
    #endregion // serialization

    #region deserialization
    private static unsafe object DeserializeArray(Type t, byte* buf, int size)
    {
        throw new NotImplementedException();
    }

    public static unsafe object? DeserializeStructure(Type t, byte* buf, int size)
    {
        // if T is a primitive type, we can just convert straight away
        if (t.IsPrimitive)
            return Marshal.PtrToStructure((IntPtr)buf, t);

        // if T is an array, use the array deserialization
        if (t.IsArray)
            return DeserializeArray(t, buf, size);

        // if T isn't a value type, we don't support it
        if (t.IsValueType == false)
            throw new NotSupportedException($"trying to deserialize reference type '{t.FullName}', reference types are unsupported due to their unknown size.");

        // create an instance of the object using the activator
        object? obj = Activator.CreateInstance(t);
        if (obj == null) return null;

        // loop through each field in T, and call ourselves.
        int i = 0;
        foreach (FieldInfo f in t.GetFields(BINDING_FLAGS))
        {
            // get the structure size of this field
            int s = GetStructureSize(f.FieldType, null);                  // ISSUE: we do not have access to the object's instance because we are creating it.

            // add the index to the buffer pointer to offset it, thus reading from another part of the array
            object? val = DeserializeStructure(f.FieldType, &buf[i], s);
            f.SetValue(obj, val);
            i += s;
        }

        return obj;
    }
    #endregion
}
