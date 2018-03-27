using System.Collections.Generic;
using Assets.Scripts.MapUtilities;
using UnityEngine;

namespace Assets.Scripts.MapChanging
{
    public interface IMapChanger
    {
        void ColorTileExact(int x, int y, Color color);
        void ColorTile(int x, int y, Color color);

        void BuildMountain(int x, int y, int levelsCount, int ringWidth, float ringHeight = 1f);
        void BuildHollow(int x, int y, int levelsCount, int ringWidth, float ringHeight = 1f);
        List<Tile> GetNeighbours(Tile tile,  NeighbourMode neighbourMode = NeighbourMode.Orthogonal);

        Dictionary<Vector2Int, Tile> Tiles { get; set; }
        int ZResolution { get; }
        int XResolution { get; }
        void RaiseTile(int x, int y, float height);
        void ChangeTileHeight(int x, int y, float height);

        void ColorRingsAround(int x, int y, int ringsCount, int ringWidth);
        HashSet<Vector2Int> GetCircle(int x, int y, int radius);
    }
}