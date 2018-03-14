using Assets.Scripts;
using NUnit.Framework;
using UnityEngine;

namespace MapGeneratorTests
{
    [TestFixture]
    public class MountainBuilderTests
    {
        [Test]
        public void BuildingOneTileMountain_ShallRaiseOnlyPeak()
        {
            var result = MountainBuilder.BuildMountain(4, 4, 1, 0);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1f, result[new Vector2Int(4,4)]);
        }

        [Test]
        public void BuildingOneTileMountain_WithRingWidth1_ShallRaisePeakAndClosestNeighbours()
        {
            var result = MountainBuilder.BuildMountain(4, 4, 1, 1);

            Assert.AreEqual(9, result.Count);

            for (int i = 3; i <= 4; ++i)
            {
                for (int j = 3; j <= 4; j++)
                {
                    Assert.AreEqual(1f, result[new Vector2Int(i,j)]);
                }
            }
        }

        [Test]
        public void BuildingOneTileMountain_WithRingWidth2__ShallRaisePeakAndClosestNeighbours()
        {
            var result = MountainBuilder.BuildMountain(4, 4, 1, 2);

            Assert.AreEqual(9, result.Count);

            for (int i = 3; i <= 4; ++i)
            {
                for (int j = 3; j <= 4; j++)
                {
                    Assert.AreEqual(1f, result[new Vector2Int(i, j)]);
                }
            }
        }


        [Test]
        public void BuildingMountainWithPeakAt2_AndRingsOfWidth2_ShallGetProperTiles()
        {
            var result = MountainBuilder.BuildMountain(4, 4, 2, 2);

            Assert.AreEqual(45, result.Count);

            for (int i = 1; i <= 7; ++i)
            {
                for (int j = 1; j <= 7; j++)
                {
                    if ((i == 1 && j == 1) || (i == 7 && j == 7) || (i == 1 && j == 7) || (i == 7 && j == 1))
                    {
                        continue;
                    }
                    var height = i >= 3 && i <= 5 && j >= 3 && j <= 5 ? 2f : 1f;
                    Assert.AreEqual(height, result[new Vector2Int(i, j)]);
                }
            }
        }
    }
}