using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapChanging
{
    public class MountainBuilder
    {
        public static Dictionary<Vector2Int, float> BuildMountain(int x, int y, int levelsCount, int ringWidth, float ringHeight, int maxX, int maxZ)
        {
            var result = new Dictionary<Vector2Int, float>();

            var ringsToBeRaised = MapPiecesSelector.GetRingsAround(x, y, levelsCount, ringWidth, maxX, maxZ);
            for(int i = 0; i < levelsCount; ++i)
            {

                foreach (var tile in ringsToBeRaised[i])
                {
                    if (MapOperationValidator.IsValidPointOnMap(tile.x, tile.y, maxX, maxZ))
                    {
                        if (!result.ContainsKey(tile))
                        {
                            result.Add(tile, (levelsCount  - i)* ringHeight);
                        }
                        else
                        {
                            result[tile] = (levelsCount - i) * ringHeight;
                        }
                    }
                }
            }
            return result;
        }

        public static Dictionary<Vector2Int, float> BuildHollow(int x, int y, int levelsCount, int ringWidth, float ringHeight, int maxX, int maxZ)
        {
            var result = new Dictionary<Vector2Int, float>();

            var ringsToBeLowered = MapPiecesSelector.GetRingsAround(x, y, levelsCount, ringWidth, maxX, maxZ);
            for (int i = 0; i < levelsCount; ++i)
            {

                foreach (var tile in ringsToBeLowered[i])
                {
                    if (MapOperationValidator.IsValidPointOnMap(tile.x, tile.y, maxX, maxZ))
                    {
                        if (!result.ContainsKey(tile))
                        {
                            result.Add(tile, (-levelsCount + i) * ringHeight);
                        }
                        else
                        {
                            result[tile] = (-levelsCount + i) * ringHeight;
                        }
                    }
                }
            }
            return result;
        }
    }
}
