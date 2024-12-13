using System;
using System.Runtime.InteropServices;

namespace Test;

public class Tests
{
    // testing a lot of the integer types, first testing alternating bit patterns with the unsigned variants of the bits
    // then testing having all bits 'true'
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { (byte)0x55, true };
        yield return new object[] { (ushort)0x5555, true };
        yield return new object[] { (uint)0x55555555, true };
        yield return new object[] { (ulong)0x5555555555555555, true };
        yield return new object[] { (sbyte)-1, true };
        yield return new object[] { (short)-1, true };
        yield return new object[] { (int)-1, true };
        yield return new object[] { (long)-1, true };
        yield return new object[] { new StructureWithPrivateFields(), true };
        yield return new object[] { new StructureWithPrivateFieldsAndStructs(), true };
        yield return new object[] { new StructureWithPublicFields(), true };
        yield return new object[] { new StructureWithPublicFieldsAndStructs(), true };
        yield return new object[] { new int[10] { int.MaxValue, 0, int.MaxValue, 1, int.MaxValue, 2, int.MaxValue, 3, int.MaxValue, 4 }, true };
        yield return new object[] { new StructureWithArray(), true };
        yield return new object[] { new StructureWithNonSerialized(0x55555555, 0x55555555), false };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Serialize_Deserialize_Equality<T>(T x, bool equal)
    {
        byte[] res = BinarySerializer.Serialize(x);
        T? y = BinarySerializer.Deserialize<T>(res);

        if (equal) Assert.Equal(x, y);
        else Assert.NotEqual(x, y);
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Serialize<T>(T x, bool equal)
    {
        _ = equal;
        _ = BinarySerializer.Serialize(x);
    }

    private struct StructureWithPublicFields
    {
        public int i1 = default;
        public int i2 = default;
        public bool b1 = true;
        public bool b2 = false;

        public StructureWithPublicFields() { }
    }

    private struct StructureWithPublicFieldsAndStructs
    {
        public StructureWithPublicFields s = default;
        public DateTime time = DateTime.Now;
        public int x = int.MaxValue;

        public StructureWithPublicFieldsAndStructs() { }
    }

    public struct StructureWithNonSerialized
    {
        public int x = 0;

        [NonSerialized]
        public int y = 0;

        public StructureWithNonSerialized(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct StructureWithArray : IEquatable<StructureWithArray>
    {
        public int[] array = [1, 2, 3, 4, 5, 6, 7, 8, 9, 0];

        public StructureWithArray() { }


        public bool Equals(StructureWithArray other)
        {
            return array.SequenceEqual(other.array);
        }
    }

#pragma warning disable CS0414 // remove unused fields

    private struct StructureWithPrivateFields
    {
        private int i1 = int.MaxValue;
        private int i2 = int.MinValue;
        private bool b1 = true;
        private bool b2 = false;

        public StructureWithPrivateFields() { }
    }

    private struct StructureWithPrivateFieldsAndStructs
    {
        private StructureWithPrivateFields s = default;
        private DateTime time = DateTime.Now;
        private int x = int.MaxValue;

        public StructureWithPrivateFieldsAndStructs() { }
    }
}
