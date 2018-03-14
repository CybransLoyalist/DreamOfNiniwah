using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace MapGeneratorTests
{
    public class MapGeneratorBaseTest
    {
        protected void AssertContainsVector(List<Vector2Int> list, Vector2Int vector2Int)
        {
            Assert.True(list.Any(a => a.x == vector2Int.x && a.y == vector2Int.y));
        }
    }
}