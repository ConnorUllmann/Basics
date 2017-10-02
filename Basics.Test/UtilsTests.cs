using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using Basics;

namespace Basics.Test
{
    public class UtilsTests
    {
        #region SequenceEqual
        private static List<List<int>> SequenceEqualTestList() =>
            new List<List<int>>()
            {
                new List<int>() { 1, 2, 3 },
                new List<int>() { 4, 5 },
                new List<int>() { 6 },
                null
            };

        [Fact] public void SequenceEqual_ReturnSucceed_DuplicateLists()
        {
            var a = SequenceEqualTestList();
            var b = SequenceEqualTestList();
            Assert.True(a.SequenceEqual(b));
            Assert.True(b.SequenceEqual(a));
        }

        [Fact] public void SequenceEqual_ReturnSucceed_DifferentLength_FirstIndex()
        {
            var a = SequenceEqualTestList();
            var b = SequenceEqualTestList();
            b.RemoveAt(0);
            Assert.False(a.SequenceEqual(b));
            Assert.False(b.SequenceEqual(a));
        }

        [Fact] public void SequenceEqual_ReturnSucceed_DifferentLength_SecondIndex()
        {
            var a = SequenceEqualTestList();
            var b = SequenceEqualTestList();
            b[0].RemoveAt(0);
            Assert.False(a.SequenceEqual(b));
            Assert.False(b.SequenceEqual(a));
        }

        [Fact] public void SequenceEqual_ReturnSucceed_DifferentValue()
        {
            var a = SequenceEqualTestList();
            var b = SequenceEqualTestList();
            b[1][1]++;
            Assert.False(a.SequenceEqual(b));
            Assert.False(b.SequenceEqual(a));
        }

        [Fact] public void SequenceEqual_ReturnSucceed_Null()
        {
            Assert.False(SequenceEqualTestList().SequenceEqual(null));
        }
        #endregion

        #region Batchify
        [Fact] public void Batchify_ReturnSucceed_EvenDivision()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5, 6 };
            var expected = new List<List<int>>() { new List<int>() { 1, 2, 3 }, new List<int>() { 4, 5, 6 } };
            var actual = a.Batchify(3);
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact] public void Batchify_ReturnSucceed_UnevenDivision()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            var expected = new List<List<int>>() { new List<int>() { 1, 2, 3 }, new List<int>() { 4, 5, 6 }, new List<int>() { 7 } };
            var actual = a.Batchify(3);
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact] public void Batchify_ReturnSucceed_BatchSizeLargerThanListLength()
        {
            var a = new List<int>() { 1, 2, 3 };
            var expected = new List<List<int>>() { new List<int>() { 1, 2, 3 } };
            var actual = a.Batchify(4);
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact] public void Batchify_ReturnSucceed_EmptyList()
        {
            var a = new List<int>() { };
            var expected = new List<List<int>>() { };
            var actual = a.Batchify(4);
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact] public void Batchify_ThrowSucceed_ZeroBatch()
        {
            Assert.Throws<ArgumentException>(() => new List<int>() { 1, 2, 3 }.Batchify(0));
        }

        [Fact] public void Batchify_ThrowSucceed_NegativeBatch()
        {
            Assert.Throws<ArgumentException>(() => new List<int>() { 1, 2, 3 }.Batchify(-2));
        }
        #endregion

        #region ExtractBatch
        [Fact] public void ExtractBatch_ReturnSucceed_Simple()
        {
            Assert.True(new List<int>() { 1, 2, 3 }.SequenceEqual(new List<int>() { 1, 2, 3, 4, 5 }.ExtractBatch(3)));
        }

        [Fact] public void ExtractBatch_ReturnSucceed_EvenDivision()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5, 6 };
            Assert.True(new List<int>() { 1, 2, 3 }.SequenceEqual(a.ExtractBatch(3)));
            Assert.True(new List<int>() { 4, 5, 6 }.SequenceEqual(a.ExtractBatch(3)));
        }

        [Fact] public void ExtractBatch_ReturnSucceed_UnevenDivision()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5 };
            Assert.True(new List<int>() { 1, 2, 3 }.SequenceEqual(a.ExtractBatch(3)));
            Assert.True(new List<int>() { 4, 5 }.SequenceEqual(a.ExtractBatch(3)));
        }

        [Fact] public void ExtractBatch_ReturnSucceed_BatchSizeLargerThanListLength()
        {
            Assert.True(new List<int>() { 1, 2, 3, 4, 5 }.SequenceEqual(new List<int>() { 1, 2, 3, 4, 5 }.ExtractBatch(8)));
        }

        [Fact] public void ExtractBatch_ReturnSucceed_ZeroBatch()
        {
            Assert.True(new List<int>().SequenceEqual(new List<int>() { 1, 2, 3 }.ExtractBatch(0)));
        }

        [Fact] public void ExtractBatch_ReturnSucceed_EmptyList()
        {
            Assert.True(new List<int>().SequenceEqual(new List<int>().ExtractBatch(5)));
        }

        [Fact] public void ExtractBatch_ModifyListSucceed_Simple()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5, 6 };
            a.ExtractBatch(3);
            Assert.True(new List<int>() { 4, 5, 6 }.SequenceEqual(a));
        }

        [Fact] public void ExtractBatch_ModifyListSucceed_EvenDivision()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5, 6 };
            a.ExtractBatch(3);
            Assert.True(new List<int>() { 4, 5, 6 }.SequenceEqual(a));
            a.ExtractBatch(3);
            Assert.True(new List<int>().SequenceEqual(a));
        }

        [Fact] public void ExtractBatch_ModifyListSucceed_UnevenDivision()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5 };
            a.ExtractBatch(3);
            Assert.True(new List<int>() { 4, 5 }.SequenceEqual(a));
            a.ExtractBatch(3);
            Assert.True(new List<int>().SequenceEqual(a));
        }

        [Fact] public void ExtractBatch_ModifyListSucceed_BatchSizeLargerThanListLength()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5 };
            a.ExtractBatch(8);
            Assert.True(new List<int>().SequenceEqual(a));
        }

        [Fact] public void ExtractBatch_ModifyListSucceed_ZeroBatch()
        {
            var a = new List<int>() { 1, 2, 3, 4, 5, 6 };
            a.ExtractBatch(0);
            Assert.True(new List<int>() { 1, 2, 3, 4, 5, 6 }.SequenceEqual(a));
        }

        [Fact] public void ExtractBatch_ModifyListSucceed_EmptyList()
        {
            var a = new List<int>();
            a.ExtractBatch(5);
            Assert.True(new List<int>().SequenceEqual(a));
        }

        [Fact] public void ExtractBatch_ThrowSucceed_NegativeBatch()
        {
            Assert.Throws(typeof(ArgumentException), () => new List<int>() { 1, 2, 3 }.ExtractBatch(-2));
        }
        #endregion

        #region HashSet.ForEach
        [Fact] public void HashSetForeach_Succeed_Simple()
        {
            var x = new HashSet<int>() { 1, 2, 3, 4, 5 };
            var y = 0;
            x.ForEach(a => y += a);
            Assert.Equal(15, y);
        }

        [Fact] public void HashSetForeach_Succeed_Empty()
        {
            var x = new HashSet<int>() { };
            var y = 0;
            x.ForEach(a => y += a);
            Assert.Equal(0, y);
        }
        #endregion

        #region AngleDifference
        [Fact] public void AngleDifferenceRadians_ReturnSucceed_PositiveBound()
        {
            Assert.Equal(Math.PI, Utils.AngleDifferenceRadians(Math.PI, Math.PI * 2));
        }

        [Fact] public void AngleDifferenceRadians_ReturnSucceed_NegativeWrap()
        {
            Assert.Equal(Math.PI, Utils.AngleDifferenceRadians(Math.PI * 2, Math.PI));
        }

        [Fact] public void AngleDifferenceRadians_ReturnSucceed_PostiveRange()
        {
            Assert.Equal(Math.PI - 1, Utils.AngleDifferenceRadians(Math.PI, Math.PI * 2 - 1));
        }

        [Fact] public void AngleDifferenceRadians_ReturnSucceed_NegativeRange()
        {
            Assert.Equal(-Math.PI + 1, Utils.AngleDifferenceRadians(Math.PI * 2 - 1, Math.PI));
        }

        [Fact] public void AngleDifferenceRadians_ReturnSucceed_LargePositiveDifference()
        {
            Assert.Equal(Math.Round(Math.PI * 0.5f, 3), Math.Round(Utils.AngleDifferenceRadians(Math.PI, Math.PI * 7.5f), 3));
        }

        [Fact] public void AngleDifferenceRadians_ReturnSucceed_LargeNegativeDifference()
        {
            Assert.Equal(Math.Round(-Math.PI * 0.5f, 3), Math.Round(Utils.AngleDifferenceRadians(Math.PI * 7.5f, Math.PI), 3));
        }
        #endregion

        #region Clamp
        [Fact] public void Clamp_List_MaxSucceed()
        {
            var a = new List<int>() { 1, 3, 5, 7, 3 };
            a.Clamp(0, 4);
            Assert.True(a.SequenceEqual(new List<int>() { 1, 3, 4, 4, 3 }));
        }

        [Fact] public void Clamp_List_MinSucceed()
        {
            var a = new List<int>() { 1, 3, 5, 7, 3 };
            a.Clamp(4, 8);
            Assert.True(a.SequenceEqual(new List<int>() { 4, 4, 5, 7, 4 }));
        }

        [Fact] public void Clamp_List_InRangeSucceed()
        {
            var a = new List<int>() { 1, 3, 5, 7, 3 };
            a.Clamp(0, 8);
            Assert.True(a.SequenceEqual(new List<int>() { 1, 3, 5, 7, 3 }));
        }

        [Fact] public void Clamp_Int_MaxSucceed()
        {
            Assert.Equal(2, Utils.Clamp(6, -2, 2));
        }

        [Fact] public void Clamp_Int_MinSucceed()
        {
            Assert.Equal(-2, Utils.Clamp(-6, -2, 2));
        }

        [Fact] public void Clamp_Int_InRangeSucceed()
        {
            Assert.Equal(0, Utils.Clamp(0, -2, 2));
        }

        [Fact] public void Clamp_Char_MaxSucceed()
        {
            Assert.Equal('y', Utils.Clamp('z', 'a', 'y'));
        }

        [Fact] public void Clamp_Char_MinSucceed()
        {
            Assert.Equal('b', Utils.Clamp('a', 'b', 'z'));
        }

        [Fact] public void Clamp_Char_InRangeSucceed()
        {
            Assert.Equal('m', Utils.Clamp('m', 'a', 'z'));
        }
        #endregion

        #region Max
        [Fact] public void Max_Int_PositiveSucceed()
        {
            Assert.Equal(8, Utils.Max(3, 1, 4, -5, 8));
        }

        [Fact] public void Max_Int_NegativeSucceed()
        {
            Assert.Equal(-1, Utils.Max(-3, -1, -4, -5, -8));
        }

        [Fact] public void Max_Char_Succeed()
        {
            Assert.Equal('d', Utils.Max('d', 'c', 'a', 'b'));
        }
        #endregion

        #region Min
        [Fact] public void Min_PositiveSucceed()
        {
            Assert.Equal(1, Utils.Min(3, 1, 4, 5, 8));
        }

        [Fact] public void Min_NegativeSucceed()
        {
            Assert.Equal(-8, Utils.Min(-3, -1, -4, -5, -8));
        }

        [Fact] public void Min_Char_Succeed()
        {
            Assert.Equal('a', Utils.Min('d', 'c', 'a', 'b'));
        }
        #endregion

        #region Distance
        [Fact] public void ManhattanDistance_ReturnSucceed_Positive()
        {
            Assert.Equal(6, Utils.ManhattanDistance(1, 2, 4, 5));
        }

        [Fact] public void ManhattanDistance_ReturnSucceed_Negative()
        {
            Assert.Equal(12, Utils.ManhattanDistance(1, -2, -4, 5));
        }

        [Fact] public void ManhattanDistance_ReturnSucceed_Zero()
        {
            Assert.Equal(0, Utils.ManhattanDistance(1, -2, 1, -2));
        }

        [Fact] public void EuclideanDistance_ReturnSucced_Positive()
        {
            Assert.Equal(Math.Round(Math.Sqrt(18), 3), Math.Round(Utils.EuclideanDistance(1, 2, 4, 5), 3));
        }

        [Fact] public void EuclideanDistance_ReturnSucced_Negative()
        {
            Assert.Equal(Math.Round(Math.Sqrt(74), 3), Math.Round(Utils.EuclideanDistance(1, -2, -4, 5), 3));
        }

        [Fact] public void EuclideanDistance_ReturnSucced_Zero()
        {
            Assert.Equal(0, Utils.EuclideanDistance(1, -2, 1, -2));
        }
        #endregion

        #region PrependToLength
        [Fact] public void PrependToLength_ReturnSucceed_Simple()
        {
            Assert.Equal("..00", Utils.PrependToLength("00", 4, "."));
        }

        [Fact] public void PrependToLength_ReturnSucceed_LongBaseString()
        {
            Assert.Equal("00000", Utils.PrependToLength("00000", 4, "."));
        }

        [Fact] public void PrependToLength_ReturnSucceed_EmptyBaseString()
        {
            Assert.Equal("....", Utils.PrependToLength("", 4, "."));
        }

        [Fact] public void PrependToLength_ReturnSucceed_NegativeLength()
        {
            Assert.Equal("00", Utils.PrependToLength("00", -4, "."));
        }

        [Fact] public void PrependToLength_ThrowSucceed_NullBaseString()
        {
            Assert.Throws<ArgumentNullException>(() => Utils.PrependToLength(null, 4, ".."));
        }

        [Fact] public void PrependToLength_ThrowSucceed_NullPrefix()
        {
            Assert.Throws<ArgumentNullException>(() => Utils.PrependToLength("00", 4, null));
        }

        [Fact] public void PrependToLength_ThrowSucceed_EmptyPrefix()
        {
            Assert.Throws<ArgumentException>(() => Utils.PrependToLength("00", 4, ""));
        }
        #endregion

        #region IsEven / IsOdd
        [Fact] public void IsEven_ReturnSucceed_PositiveEven()
        {
            Assert.True(4.IsEven());
        }

        [Fact] public void IsEven_ReturnSucceed_PositiveOdd()
        {
            Assert.False(5.IsEven());
        }

        [Fact] public void IsEven_ReturnSucceed_Zero()
        {
            Assert.True(0.IsEven());
        }

        [Fact] public void IsEven_ReturnSucceed_NegativeEven()
        {
            Assert.True((-6).IsEven());
        }

        [Fact] public void IsEven_ReturnSucceed_NegativeOdd()
        {
            Assert.False((-7).IsEven());
        }

        [Fact] public void IsOdd_ReturnSucceed_PositiveEven()
        {
            Assert.False(4.IsOdd());
        }

        [Fact] public void IsOdd_ReturnSucceed_PositiveOdd()
        {
            Assert.True(5.IsOdd());
        }

        [Fact] public void IsOdd_ReturnSucceed_Zero()
        {
            Assert.False(0.IsOdd());
        }

        [Fact] public void IsOdd_ReturnSucceed_NegativeEven()
        {
            Assert.False((-6).IsOdd());
        }

        [Fact] public void IsOdd_ReturnSucceed_NegativeOdd()
        {
            Assert.True((-7).IsOdd());
        }
        #endregion

        #region
        [Fact] public void AddOrUpdate_ModifySucceed_SingleAdd()
        {
            var dict = new Dictionary<int, int>();
            dict.AddOrUpdate(1, 10);
            Assert.Equal(1, dict.Count);
            Assert.True(dict.ContainsKey(1));
            Assert.Equal(10, dict[1]);
        }

        [Fact] public void AddOrUpdate_ModifySucceed_MultipleAdds()
        {
            var dict = new Dictionary<int, int>();
            dict.AddOrUpdate(1, 10);
            dict.AddOrUpdate(2, 20);
            Assert.Equal(2, dict.Count);
            Assert.True(dict.ContainsKey(1));
            Assert.True(dict.ContainsKey(2));
            Assert.Equal(10, dict[1]);
            Assert.Equal(20, dict[2]);
        }

        [Fact] public void AddOrUpdate_ModifySucceed_SingleUpdate()
        {
            var dict = new Dictionary<int, int>();
            dict.AddOrUpdate(1, 10);
            dict.AddOrUpdate(2, 20);

            dict.AddOrUpdate(1, 100);

            Assert.True(dict.ContainsKey(1));
            Assert.True(dict.ContainsKey(2));
            Assert.Equal(100, dict[1]);
            Assert.Equal(20, dict[2]);
        }

        [Fact] public void AddOrUpdate_ModifySucceed_MultipleUpdates()
        {
            var dict = new Dictionary<int, int>();
            dict.AddOrUpdate(1, 10);
            dict.AddOrUpdate(2, 20);

            dict.AddOrUpdate(1, 100);
            dict.AddOrUpdate(2, 200);
            dict.AddOrUpdate(1, 1000);

            Assert.True(dict.ContainsKey(1));
            Assert.True(dict.ContainsKey(2));
            Assert.Equal(1000, dict[1]);
            Assert.Equal(200, dict[2]);
        }
        #endregion
    }
}
