using UnityEngine;

namespace Assets.Scripts
{
    public interface IMap
    {
        void ColorTileExact(int x, int y, Color color);
        void ColorTile(int x, int y, Color color);

        void BuildMountain(int x, int y, int peakHeigh, int ringWidth);
        void BuildHollow(int x, int y, int peakHeigh, int ringWidth);
    }
}