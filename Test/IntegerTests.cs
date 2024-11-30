using System.Runtime.InteropServices;

namespace Test;

public class IntegerTests
{
    // testing a lot of the integer types, first testing alternating bit patterns with the unsigned variants of the bits
    // then testing having all bits 'true'
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { (byte)0x55, (ulong)0x55 };
        yield return new object[] { (ushort)0x5555, (ulong)0x5555 };
        yield return new object[] { (uint)0x55555555, (ulong)0x55555555 };
        yield return new object[] { (ulong)0x5555555555555555, (ulong)0x5555555555555555 };
        yield return new object[] { (sbyte)-1, unchecked((ulong)-1) };
        yield return new object[] { (short)-1, unchecked((ulong)-1) };
        yield return new object[] { (int)-1, unchecked((ulong)-1) };
        yield return new object[] { (long)-1, unchecked((ulong)-1) };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Serialize_Deserialize_Equality<T>(T x) where T : struct
    {
        byte[] res = BinarySerializer.Serialize(x);
        T? y = BinarySerializer.Deserialize<T>(res);

        Assert.Equal(x, y);
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Serialize<T>(T x, ulong val) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] expRes = new byte[size];

        for (int i = 0; i < size; i++)
        {
            expRes[i] = (byte)(val >> (i * 8)); // casting to 'byte' truncates the rest
        }

        byte[] res = BinarySerializer.Serialize(x);
        Assert.Equal(res, expRes);
    }
}
