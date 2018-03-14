using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{

    public class MountainBuilder
    {
        public static Dictionary<Vector2Int, float> BuildMountain(int x, int y, int peakHeight, int ringWidth)
        {
            var result = new Dictionary<Vector2Int, float>();

            //  RaiseTile(x, y, peakHeight);
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
                var currentRingWidth = level == 1 ? ringWidth - 1 : ringWidth;
                var ringToBeRaised = MapPiecesSelector.GetRingAround(x, y, ringBeginning, ringWidth);
                foreach (var tile in ringToBeRaised)
                {
                    result.Add(tile, level);
                }
                ringBeginning += ringWidth;
                level--;
            }
            return result;

            // var terracesWidth = (int)Mathf.Ceil((float) radius / (float) peakHeight);
            //            var terracesCount = peakHeight - 1;
            //            if (terracesCount == 0)
            //            {
            //                return;
            //            }
            //            for (int i = 1; i <= terracesCount; i++)
            //            {
            //                print("building " + i + "th tarrace. level = " + level);
            //                for (int ring = (i * ringWidth); ring < ((i + 1) * ringWidth); ring++)
            //                {
            //                    print("ring at" + ring);
            //                    print("height" + (peakHeight - level - 1));
            //                    var ringAround = MapPiecesSelector.GetRingAround(x, y, ring, ringWidth);
            //                    foreach (var tile in ringAround)
            //                    {
            //                        // ColorTileExact(tile.x, tile.y, Color.magenta);
            //                        ;
            //                        RaiseTile(tile.x, tile.y, peakHeight - level - 1);
            //                    }
            //                    break;
            //                }
            //                break;
            //                ++level;
            //            }
            // }
        }
    }
}
