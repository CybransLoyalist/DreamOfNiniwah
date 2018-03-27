using System;

namespace Assets.Scripts.RandomMapGenerating
{
    public enum Side
    {
        Top,
        Right,
        Bottom,
        Left
    }

    public static class SideExtensions
    {
        public static Side GetOpposite(this Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return Side.Bottom;
                case Side.Right:
                    return Side.Left;
                case Side.Bottom:
                    return Side.Top;
                case Side.Left:
                    return Side.Right;
                default:
                    throw new ArgumentOutOfRangeException("side", side, null);
            }
        }
    }
}