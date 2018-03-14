namespace Assets.Scripts
{
    public interface IMap
    {
        void BuildMountain(int x, int y, int peakHeigh, int ringWidth);
        void SetYPositionOfMiddleVertices();
        void CommitChanges();
    }
}