using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapChanging
{
    public class MapPiecesSelector : MonoBehaviour
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

        public static Dictionary<int, List<Vector2Int>> GetRingsAround(int x, int y, int ringsCount, int ringWidth,
            int maxX, int maxZ, bool shortenLastRingByOne = true)
        {
            var result = new Dictionary<int, List<Vector2Int>>();

            for (int i = 0; i < ringsCount + 1; i++)
            {
                result[i] = new List<Vector2Int>();
                int currentRingOuterRadius = (i + 1) * ringWidth;
                var currentRingInnerRadius = currentRingOuterRadius - ringWidth;

                for (int currentY = 0; currentY < currentRingOuterRadius; ++currentY)
                {
                    var mostOuterX =
                        (int) (Mathf.RoundToInt(Mathf.Sqrt(
                            Mathf.Abs(currentRingOuterRadius * currentRingOuterRadius - currentY * currentY))));

                    int mostInnerX;
                    if (currentRingInnerRadius < currentY)
                    {
                        mostInnerX = 0;
                    }
                    else
                    {
                        mostInnerX =
                            (int) (Mathf.RoundToInt(
                                Mathf.Sqrt(Mathf.Abs(currentRingInnerRadius * currentRingInnerRadius -
                                                     currentY * currentY))));
                    }

                    if (i == ringsCount - 1 && shortenLastRingByOne)
                    {
                        mostOuterX--;
                    }

                    for (int currentX = mostInnerX; currentX < mostOuterX; currentX++)
                    {
                        result[i].Add(new Vector2Int(currentX, currentY));
                    }
                }
            }

            var finalResult = new Dictionary<int, List<Vector2Int>>();
            for (int i = 0; i < ringsCount + 1; i++)
            {
                finalResult[i] = new List<Vector2Int>();
                var toBeCopied = result[i].Select(a => a).ToList();
                foreach (var vector2Int in toBeCopied)
                {
                    finalResult[i].Add(new Vector2Int(x + vector2Int.x, y + vector2Int.y));
                    finalResult[i].Add(new Vector2Int(x - vector2Int.x, y + vector2Int.y));
                    finalResult[i].Add(new Vector2Int(x + vector2Int.x, y - vector2Int.y));
                    finalResult[i].Add(new Vector2Int(x - vector2Int.x, y - vector2Int.y));
                }
            }

            return finalResult;
        }
    }
}