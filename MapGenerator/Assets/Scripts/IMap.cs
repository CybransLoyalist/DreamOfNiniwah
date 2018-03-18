using UnityEngine;

namespace Assets.Scripts
{
    public interface IMap
    {
        void BuildMountain(int x, int y, int peakHeigh, int ringWidth);
//        void BuildHollow(int x, int y, int peakHeigh, int ringWidth);
//
        void CommitChanges();
        void ColorTileExact(int x, int y, Color color);

       void RaiseTile(int x, int y, float height);
//
        void LowerTile(int x, int y, float height);
        void ColorTile(int x, int y, Color color);
    }
}