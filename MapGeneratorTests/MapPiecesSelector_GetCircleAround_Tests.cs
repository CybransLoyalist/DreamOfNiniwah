using System;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using Assets.Scripts.MapChanging;
using NUnit.Framework;
using UnityEngine;

namespace MapGeneratorTests
{
    [TestFixture]
    public class MapPiecesSelector_GetCircleAround_Tests : MapGeneratorBaseTest
    {
        private const int MaxX = 20;
        private const int MaxZ = 20;

        [Test]
        public void GetZeroCircleAround_ShallGetCenterTile()
        {
            var result = MapPiecesSelector.GetCircleAround(4, 4, 0, MaxX, MaxZ);

            Assert.AreEqual(1, result.Count);
            
            AssertContainsVector(result, new Vector2Int(4,4));
        }

        [Test]
        public void GetFirstCircleAround_ShallGetProperTiles()
        {
            var result = MapPiecesSelector.GetCircleAround(4, 4, 1, MaxX, MaxZ);

            Assert.AreEqual(9, result.Count);

            AssertContainsVector(result, new Vector2Int(3,3));
            AssertContainsVector(result, new Vector2Int(4,3));
            AssertContainsVector(result, new Vector2Int(5,3));

            AssertContainsVector(result, new Vector2Int(3,4));
            AssertContainsVector(result, new Vector2Int(4,4));
            AssertContainsVector(result, new Vector2Int(5,4));

            AssertContainsVector(result, new Vector2Int(3,5));
            AssertContainsVector(result, new Vector2Int(4,5));
            AssertContainsVector(result, new Vector2Int(5,5));
        }

        [Test]
        public void GetSecondCircleAround_ShallGetProperTiles()
        {
            var result = MapPiecesSelector.GetCircleAround(4, 4, 2, MaxX, MaxZ);

            Assert.AreEqual(25, result.Count);

            AssertContainsVector(result, new Vector2Int(2,2));
            AssertContainsVector(result, new Vector2Int(3,2));
            AssertContainsVector(result, new Vector2Int(4,2));
            AssertContainsVector(result, new Vector2Int(5,2));
            AssertContainsVector(result, new Vector2Int(6,2));

            AssertContainsVector(result, new Vector2Int(2,3));
            AssertContainsVector(result, new Vector2Int(3,3));
            AssertContainsVector(result, new Vector2Int(4,3));
            AssertContainsVector(result, new Vector2Int(5,3));
            AssertContainsVector(result, new Vector2Int(6,3));

            AssertContainsVector(result, new Vector2Int(2,4));
            AssertContainsVector(result, new Vector2Int(3,4));
            AssertContainsVector(result, new Vector2Int(4,4));
            AssertContainsVector(result, new Vector2Int(5,4));
            AssertContainsVector(result, new Vector2Int(6,4));

            AssertContainsVector(result, new Vector2Int(2,5));
            AssertContainsVector(result, new Vector2Int(3,5));
            AssertContainsVector(result, new Vector2Int(4,5));
            AssertContainsVector(result, new Vector2Int(5,5));
            AssertContainsVector(result, new Vector2Int(6,5));

            AssertContainsVector(result, new Vector2Int(2,6));
            AssertContainsVector(result, new Vector2Int(3,6));
            AssertContainsVector(result, new Vector2Int(4,6));
            AssertContainsVector(result, new Vector2Int(5,6));
            AssertContainsVector(result, new Vector2Int(6,6));
        }

        [Test]
        public void GetThirdCircleAround_ShallGetProperTiles()
        {
            var result = MapPiecesSelector.GetCircleAround(4, 4, 3, MaxX, MaxZ);

            Assert.AreEqual(45, result.Count);
            
            AssertContainsVector(result, new Vector2Int(2,1));
            AssertContainsVector(result, new Vector2Int(3,1));
            AssertContainsVector(result, new Vector2Int(4,1));
            AssertContainsVector(result, new Vector2Int(5,1));
            AssertContainsVector(result, new Vector2Int(6,1));

            AssertContainsVector(result, new Vector2Int(1,2));
            AssertContainsVector(result, new Vector2Int(2,2));
            AssertContainsVector(result, new Vector2Int(3,2));
            AssertContainsVector(result, new Vector2Int(4,2));
            AssertContainsVector(result, new Vector2Int(5,2));
            AssertContainsVector(result, new Vector2Int(6,2));
            AssertContainsVector(result, new Vector2Int(7,2));

            AssertContainsVector(result, new Vector2Int(1,3));
            AssertContainsVector(result, new Vector2Int(2,3));
            AssertContainsVector(result, new Vector2Int(3,3));
            AssertContainsVector(result, new Vector2Int(4,3));
            AssertContainsVector(result, new Vector2Int(5,3));
            AssertContainsVector(result, new Vector2Int(6,3));
            AssertContainsVector(result, new Vector2Int(7,3));

            AssertContainsVector(result, new Vector2Int(1,4));
            AssertContainsVector(result, new Vector2Int(2,4));
            AssertContainsVector(result, new Vector2Int(3,4));
            AssertContainsVector(result, new Vector2Int(4,4));
            AssertContainsVector(result, new Vector2Int(5,4));
            AssertContainsVector(result, new Vector2Int(6,4));
            AssertContainsVector(result, new Vector2Int(7,4));

            AssertContainsVector(result, new Vector2Int(1,5));
            AssertContainsVector(result, new Vector2Int(2,5));
            AssertContainsVector(result, new Vector2Int(3,5));
            AssertContainsVector(result, new Vector2Int(4,5));
            AssertContainsVector(result, new Vector2Int(5,5));
            AssertContainsVector(result, new Vector2Int(6,5));
            AssertContainsVector(result, new Vector2Int(7,5));

            AssertContainsVector(result, new Vector2Int(1,6));
            AssertContainsVector(result, new Vector2Int(2,6));
            AssertContainsVector(result, new Vector2Int(3,6));
            AssertContainsVector(result, new Vector2Int(4,6));
            AssertContainsVector(result, new Vector2Int(5,6));
            AssertContainsVector(result, new Vector2Int(6,6));
            AssertContainsVector(result, new Vector2Int(7,6));
            
            AssertContainsVector(result, new Vector2Int(2,7));
            AssertContainsVector(result, new Vector2Int(3,7));
            AssertContainsVector(result, new Vector2Int(4,7));
            AssertContainsVector(result, new Vector2Int(5,7));
            AssertContainsVector(result, new Vector2Int(6,7));
        }
    }
}
