using System.Runtime.InteropServices;
using ThePigeonGenerator.Util;

namespace Test;

public class StructTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { new StructureWithPublicFields() };
        // yield return new object[] { new StructureWithPublicFieldsAndStructs() };
        yield return new object[] { new StructureWithPrivateFields() };
        // yield return new object[] { new StructureWithPrivateFieldsAndStructs() };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void Serialize_Deserialize_Struct<T>(T x) where T : struct
    {
        byte[] res = BinarySerializer.Serialize(x);
        T? y = BinarySerializer.Deserialize<T>(res);

        Assert.Equal(x, y);
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

    private struct StructureWithPrivateFields
    {
        int i1 = default;
        int i2 = default;
        bool b1 = true;
        bool b2 = false;

        public StructureWithPrivateFields() { }
    }

    private struct StructureWithPrivateFieldsAndStructs
    {
        StructureWithPrivateFields s = default;
        DateTime time = DateTime.Now;
        int x = int.MaxValue;

        public StructureWithPrivateFieldsAndStructs() { }
    }
}
