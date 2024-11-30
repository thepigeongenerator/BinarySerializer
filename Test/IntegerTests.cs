using System.Runtime.InteropServices;

namespace Test;

public class IntegerTests
{
    // testing a lot of the integer types, first testing alternating bit patterns with the unsigned variants of the bits
    // then testing having all bits 'true'
    [Theory]
    [InlineData((byte)0x55)]
    [InlineData((ushort)0x5555)]
    [InlineData((uint)0x55555555)]
    [InlineData((ulong)0x5555555555555555)]
    [InlineData((sbyte)-1)]
    [InlineData((short)-1)]
    [InlineData((int)-1)]
    [InlineData((long)-1)]
    public void Serialize_Deserialize_Equality<T>(T x) where T : struct
    {
        byte[] res = BinarySerializer.Serialize(x);
        T? y = BinarySerializer.Deserialize<T>(res);

        Assert.Equal(x, y);
    }

    [Theory]
    [InlineData(default(byte))]
    [InlineData(default(ushort))]
    [InlineData(default(uint))]
    [InlineData(default(ulong))]
    public void Serialize_Deserialize_Size<T>(T x) where T : struct
    {
        int size = Marshal.SizeOf<T>();
        byte[] res = BinarySerializer.Serialize(x);

        Assert.True(size == res.Length);
    }
}
