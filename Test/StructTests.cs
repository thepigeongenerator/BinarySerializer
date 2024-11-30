using System.Runtime.InteropServices;

namespace Test;

public class StructTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { new StructureWithPublicFields() };
        yield return new object[] { new StructureWithPublicFieldsAndStructs() };
        yield return new object[] { new StructureWithPrivateFields() };
        yield return new object[] { new StructureWithPrivateFieldsAndStructs() };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Serialize_Deserialize<T>(T x) where T : struct
    {
        byte[] res = BinarySerializer.Serialize(x);
        T? y = BinarySerializer.Deserialize<T>(res);

        Assert.Equal(x, y);
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Serialize<T>(T x) where T : struct
    {
        try
        {
            _ = BinarySerializer.Serialize(x);
        }
        catch
        {
            Assert.Fail("an error occurred when trying to serialize!");
        }
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
