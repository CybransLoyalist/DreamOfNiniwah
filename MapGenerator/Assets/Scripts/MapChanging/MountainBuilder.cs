using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapChanging
{
    public class MountainBuilder
    {
        public static Dictionary<Vector2Int, float> BuildMountain(int x, int y, int levelsCount, int ringWidth, float ringHeight, int maxX, int maxZ)
        {
            var result = new Dictionary<Vector2Int, float>();
            
            var circleAroundPeakRadius = ringWidth == 0 ? 0 : (int)Mathf.Ceil(ringWidth /2f);
            var circleAroundPeak = MapPiecesSelector.GetCircleAround(x, y, circleAroundPeakRadius, maxX, maxZ);
            foreach (var tile in circleAroundPeak)
            {
                result.Add(tile, levelsCount * ringHeight);
            }


            int level = levelsCount - 1;
            var ringBeginning = circleAroundPeakRadius + 1;

            while (level > 0)
            {
                var ringToBeRaised = MapPiecesSelector.GetRingAround(x, y, ringBeginning, ringWidth, maxX, maxZ);
                foreach (var tile in ringToBeRaised)
                {
                    if(MapOperationValidator.IsValidPointOnMap(tile.x, tile.y, maxX, maxZ))
                    {
                        result.Add(tile, level * ringHeight);
                    }
                }
                ringBeginning += ringWidth;
                level--;
            }
            return result;
        }

        public static Dictionary<Vector2Int, float> BuildHollow(int x, int y, int levelsCount, int ringWidth, float ringHeight, int maxX, int maxZ)
        {
            var result = new Dictionary<Vector2Int, float>();
            
            var circleAroundPeakRadius = ringWidth == 0 ? 0 : (int)Mathf.Ceil(ringWidth /2f);
            var circleAroundPeak = MapPiecesSelector.GetCircleAround(x, y, circleAroundPeakRadius, maxX, maxZ);
            foreach (var tile in circleAroundPeak)
            {
                result.Add(tile, levelsCount * ringHeight);
            }


            int level = levelsCount + 1;
            var ringBeginning = circleAroundPeakRadius + 1;

            while (level < 0)
            {
                var ringToBeRaised = MapPiecesSelector.GetRingAround(x, y, ringBeginning, ringWidth, maxX, maxZ);
                foreach (var tile in ringToBeRaised)
                {
                    if(MapOperationValidator.IsValidPointOnMap(tile.x, tile.y, maxX, maxZ))
                    {
                        result.Add(tile, level * ringHeight);
                    }
                }
                ringBeginning += ringWidth;
                level++;
            }
            return result;
        }
    }
}
