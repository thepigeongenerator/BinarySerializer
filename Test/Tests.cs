using ThePigeonGenerator.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace Test
{
    [TestClass]
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
            yield return new object[] { new int[10] { int.MaxValue, 0, int.MaxValue, 1, int.MaxValue, 2, int.MaxValue, 3, int.MaxValue, 4 }, true };
            yield return new object[] { "this_is_a_string", true };
            yield return new object[] { new StructureWithString(0), true };
            yield return new object[] { new StructureWithPrivateFields(0), true };
            yield return new object[] { new StructureWithPrivateFieldsAndStructs(0), true };
            yield return new object[] { new StructureWithPublicFields(0), true };
            yield return new object[] { new StructureWithPublicFieldsAndStructs(0), true };
            yield return new object[] { new StructureWithArray(0), true };
            yield return new object[] { new StructureWithNonSerialized(0x55555555, 0x55555555), false };
        }

        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        [DataTestMethod]
        public void Serialize_Deserialize_Equality(object x, bool equal)
        {
            // executes unit test
            int exec<T>(T o1)
            {
                byte[] res = BinarySerializer.Serialize(o1);
                T? o2 = BinarySerializer.Deserialize<T>(res);

                if (o1 is not ICollection c1)
                {
                    if (equal) Assert.AreEqual(o1, o2);
                    else Assert.AreNotEqual(o1, o2);
                    return 0;
                }
                else
                {
                    ICollection c2 = o2 as ICollection ?? throw new NullReferenceException();

                    if (equal) CollectionAssert.AreEqual(c1, c2);
                    else CollectionAssert.AreNotEqual(c1, c2);
                    return 0;
                }
            }

            // get which type we're checking
            _ = x switch
            {
                byte o => exec(o),
                ushort o => exec(o),
                uint o => exec(o),
                ulong o => exec(o),
                sbyte o => exec(o),
                short o => exec(o),
                int o => exec(o),
                long o => exec(o),
                int[] o => exec(o),
                string o => exec(o),
                StructureWithPrivateFields o => exec(o),
                StructureWithPrivateFieldsAndStructs o => exec(o),
                StructureWithPublicFields o => exec(o),
                StructureWithPublicFieldsAndStructs o => exec(o),
                StructureWithArray o => exec(o),
                StructureWithNonSerialized o => exec(o),
                StructureWithString o => exec(o),
                _ => throw new InvalidOperationException("found an unknown type, didn't know what to do!"),
            };
        }

        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        [DataTestMethod]
        public void Serialize(object x, bool equal)
        {
            _ = equal;
            _ = BinarySerializer.Serialize(x.GetType(), x);
        }

        private struct StructureWithPublicFields
        {
            public int i1;
            public int i2;
            public bool b1;
            public bool b2;

            public StructureWithPublicFields(int _)
            {
                i1 = default;
                i2 = default;
                b1 = true;
                b2 = false;
            }
        }

        private struct StructureWithPublicFieldsAndStructs
        {
            public StructureWithPublicFields s;
            public DateTime time;
            public int x;

            public StructureWithPublicFieldsAndStructs(int _)
            {
                s = default;
                time = DateTime.Now;
                x = int.MaxValue;
            }
        }

        public struct StructureWithNonSerialized
        {
            public int x;

            [NonSerialized]
            public int y;

            public StructureWithNonSerialized(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public struct StructureWithArray : IEquatable<StructureWithArray>
        {
            public int[] array;

            public StructureWithArray(int _)
            {
                array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            }


            public bool Equals(StructureWithArray other)
            {
                return array.SequenceEqual(other.array);
            }
        }

        public struct StructureWithString
        {
            public string str;

            public StructureWithString(int _)
            {
                str = "hello, world";
            }
        }

#pragma warning disable CS0414 // remove unused fields

        private struct StructureWithPrivateFields
        {
            private int i1;
            private int i2;
            private bool b1;
            private bool b2;

            public StructureWithPrivateFields(int _)
            {
                i1 = int.MaxValue;
                i2 = int.MinValue;
                b1 = true;
                b2 = false;
            }
        }

        private struct StructureWithPrivateFieldsAndStructs
        {
            private StructureWithPrivateFields s;
            private DateTime time;
            private int x;

            public StructureWithPrivateFieldsAndStructs(int _)
            {
                s = default;
                time = DateTime.Now;
                x = int.MaxValue;
            }
        }
    }
}
