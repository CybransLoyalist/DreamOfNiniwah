using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MountainBuilder
    {
        public static Dictionary<Vector2Int, float> BuildMountain(int x, int y, int peakHeight, int ringWidth, int maxX, int maxZ)
        {
            var result = new Dictionary<Vector2Int, float>();
            
            var circleAroundPeakRadius = ringWidth == 0 ? 0 : (int)Mathf.Ceil(ringWidth /2f);
            var circleAroundPeak = MapPiecesSelector.GetCircleAround(x, y, circleAroundPeakRadius);
            foreach (var tile in circleAroundPeak)
            {
                result.Add(tile, peakHeight);
            }


            int level = peakHeight - 1;
            var ringBeginning = circleAroundPeakRadius + 1;

            while (level > 0)
            {
                var ringToBeRaised = MapPiecesSelector.GetRingAround(x, y, ringBeginning, ringWidth);
                foreach (var tile in ringToBeRaised)
                {
                    if(MapOperationValidator.IsValidPointOnMap(tile.x, tile.y, maxX, maxZ))
                    {
                        result.Add(tile, level);
                    }
                }
                ringBeginning += ringWidth;
                level--;
            }
            return result;
        }
    }
}
