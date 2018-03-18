using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IMap
    {
        void ColorTileExact(int x, int y, Color color);
        void ColorTile(int x, int y, Color color);

        void BuildMountain(int x, int y, int peakHeigh, int ringWidth);
        void BuildHollow(int x, int y, int peakHeigh, int ringWidth);
        List<Tile> GetNeighbours(Tile tile,  NeighbourMode neighbourMode = NeighbourMode.Orthogonal);

        int ZResolution { get; }
        int XResolution { get; }
        void RaiseTile(int x, int y, float height);
    }
}