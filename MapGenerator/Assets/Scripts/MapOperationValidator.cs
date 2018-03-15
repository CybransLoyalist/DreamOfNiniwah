namespace Assets.Scripts
{
    public static class MapOperationValidator
    {
        public static bool IsValidPointOnMap(int x, int z, int maxX, int maxZ)
        {
            return x >= 0 &&x < maxX && z >= 0 && z < maxZ;
        }
    }
}