using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapChanging
{
    public class MapPiecesSelector
    {

        public static Vector2Int GetXRangeForRing(int x, int y, int ringIndex, int ringWidth)
        {
            var mostLeft = x - ringIndex - ringWidth + 1;
            var mostRight = x + ringIndex + ringWidth - 1;
            return new Vector2Int(mostLeft, mostRight);
        }

        public static Vector2Int GetYRangeForRing(int x, int y, int ringIndex, int ringWidth)
        {
            var mostTop = y - ringIndex - ringWidth + 1;
            var mostBottom = y + ringIndex + ringWidth - 1;
            return new Vector2Int(mostTop, mostBottom);
        }

        public static List<Vector2Int> GetCircleAround(int x, int y, int circleIndex, int maxX, int maxY)
        {
            var center = new Vector2Int(x, y);
            var result = new List<Vector2Int>();

            var minXNum = x - circleIndex >= 0 ? x - circleIndex : 0;
            var maxXNum = x + circleIndex < maxX ? x + circleIndex : maxX;

            var minYNum = y - circleIndex >= 0 ? y - circleIndex : 0;
            var maxYNum = y + circleIndex < maxY ? y + circleIndex : maxY;


            for (int xNum = minXNum; xNum <= maxXNum; ++xNum)
            {
                for (int yNum = minYNum; yNum <= maxYNum; ++yNum)
                {
                    var radius = Vector2Int.Distance(center, new Vector2Int(xNum, yNum));
                    if (radius <= circleIndex + 1)
                    {
                        result.Add(new Vector2Int(xNum, yNum));
                    }
                }
            }
            return result;
        }

        public static List<Vector2Int> GetRingAround(int x, int y, int ringIndex, int ringWidth, int maxX, int maxZ)
        {
            if (ringIndex == 0)
            {
                throw new ArgumentException("Can't search for ring on index 0.");
            }

            var outerCircle = GetCircleAround(x, y, ringIndex + ringWidth - 1, maxX, maxZ);
            var innerCircle = GetCircleAround(x, y, ringIndex - 1, maxX, maxZ);

            var difference = outerCircle.Except(innerCircle).ToList();
            return difference;
        }
    }
}