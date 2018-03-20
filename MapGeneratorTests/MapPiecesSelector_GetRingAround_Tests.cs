using System;
using Assets.Scripts;
using Assets.Scripts.MapChanging;
using NUnit.Framework;
using UnityEngine;

namespace MapGeneratorTests
{
    [TestFixture]
    public class MapPiecesSelector_GetRingAround_Tests : MapGeneratorBaseTest
    {
        public const int maxX = 20;
        public const int maxZ = 20;

        [Test]
        public void GetXRangeForFirstRing_OfWidth1_ShallGetProperRange_1()
        {
            var result = MapPiecesSelector.GetXRangeForRing(4, 4, 1, 1);
            Assert.AreEqual(3, result.x);
            Assert.AreEqual(5, result.y);
        }

        [Test]
        public void GetXRangeForFirstRing_OfWidth1_ShallGetProperRange_2()
        {
            var result = MapPiecesSelector.GetXRangeForRing(5, 6, 1, 1);
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(6, result.y);
        }

        [Test]
        public void GetXRangeForFirstRing_OfWidth2_ShallGetProperRange_2()
        {
            var result = MapPiecesSelector.GetXRangeForRing(5, 6, 1, 2);
            Assert.AreEqual(3, result.x);
            Assert.AreEqual(7, result.y);
        }

        [Test]
        public void GetYRangeForFirstRing_OfWidth1_ShallGetProperRange_1()
        {
            var result = MapPiecesSelector.GetYRangeForRing(4, 4, 1, 1);
            Assert.AreEqual(3, result.x);
            Assert.AreEqual(5, result.y);
        }

        [Test]
        public void GetYRangeForFirstRing_OfWidth2_ShallGetProperRange_1()
        {
            var result = MapPiecesSelector.GetYRangeForRing(4, 4, 1, 2);
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(6, result.y);
        }

        [Test]
        public void GetYRangeForFirstRing_OfWidth1_ShallGetProperRange_2()
        {
            var result = MapPiecesSelector.GetYRangeForRing(4, 3, 1, 1);
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(4, result.y);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GetZeroRingAround_ShallThrowException(int ringWidth)
        {
            Assert.Throws<ArgumentException>(() => MapPiecesSelector.GetRingAround(4, 4, 0, ringWidth, maxX, maxZ));
        }

        [Test]
        public void GetFirstRingAround_OfWidth1_ShallGetProperRing()
        {
            var result = MapPiecesSelector.GetRingAround(4, 4, 1, 1, maxX, maxZ);
            Assert.AreEqual(8, result.Count);

            AssertContainsVector(result, new Vector2Int(3,3));
            AssertContainsVector(result, new Vector2Int(4,3));
            AssertContainsVector(result, new Vector2Int(5,3));

            AssertContainsVector(result, new Vector2Int(3,4));
            AssertContainsVector(result, new Vector2Int(5,4));

            AssertContainsVector(result, new Vector2Int(3,5));
            AssertContainsVector(result, new Vector2Int(4,5));
            AssertContainsVector(result, new Vector2Int(5,5));
        }

        [Test]
        public void GetFirstRingAround_OfWidth2_ShallGetProperRing()
        {
            var result = MapPiecesSelector.GetRingAround(4, 4, 1, 2, maxX, maxZ);
            Assert.AreEqual(24, result.Count);

            AssertContainsVector(result, new Vector2Int(2, 2));
            AssertContainsVector(result, new Vector2Int(3, 2));
            AssertContainsVector(result, new Vector2Int(4, 2));
            AssertContainsVector(result, new Vector2Int(5, 2));
            AssertContainsVector(result, new Vector2Int(6, 2));

            AssertContainsVector(result, new Vector2Int(2, 3));
            AssertContainsVector(result, new Vector2Int(3, 3));
            AssertContainsVector(result, new Vector2Int(4, 3));
            AssertContainsVector(result, new Vector2Int(5, 3));
            AssertContainsVector(result, new Vector2Int(6, 3));

            AssertContainsVector(result, new Vector2Int(2, 4));
            AssertContainsVector(result, new Vector2Int(3, 4));
            AssertContainsVector(result, new Vector2Int(5, 4));
            AssertContainsVector(result, new Vector2Int(6, 4));

            AssertContainsVector(result, new Vector2Int(2, 5));
            AssertContainsVector(result, new Vector2Int(3, 5));
            AssertContainsVector(result, new Vector2Int(4, 5));
            AssertContainsVector(result, new Vector2Int(5, 5));
            AssertContainsVector(result, new Vector2Int(6, 5));

            AssertContainsVector(result, new Vector2Int(2, 6));
            AssertContainsVector(result, new Vector2Int(3, 6));
            AssertContainsVector(result, new Vector2Int(4, 6));
            AssertContainsVector(result, new Vector2Int(5, 6));
            AssertContainsVector(result, new Vector2Int(6, 6));
        }

        [Test]
        public void GetSecondRingAround_OfWidth1_ShallGetProperRing()
        {
            var result = MapPiecesSelector.GetRingAround(4, 4, 2, 1, maxX, maxZ);
            Assert.AreEqual(16, result.Count);

            AssertContainsVector(result, new Vector2Int(2, 2));
            AssertContainsVector(result, new Vector2Int(3, 2));
            AssertContainsVector(result, new Vector2Int(4, 2));
            AssertContainsVector(result, new Vector2Int(5, 2));
            AssertContainsVector(result, new Vector2Int(6, 2));

            AssertContainsVector(result, new Vector2Int(2, 3));
            AssertContainsVector(result, new Vector2Int(6, 3));

            AssertContainsVector(result, new Vector2Int(2, 4));
            AssertContainsVector(result, new Vector2Int(6, 4));

            AssertContainsVector(result, new Vector2Int(2, 5));
            AssertContainsVector(result, new Vector2Int(6, 5));

            AssertContainsVector(result, new Vector2Int(2, 6));
            AssertContainsVector(result, new Vector2Int(3, 6));
            AssertContainsVector(result, new Vector2Int(4, 6));
            AssertContainsVector(result, new Vector2Int(5, 6));
            AssertContainsVector(result, new Vector2Int(6, 6));
        }
    }
}