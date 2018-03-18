using UnityEngine;

namespace Assets.Scripts
{
    public class ProceduralInfiniteLandscapeGenerator : MonoBehaviour
    {

        [SerializeField] private MapGenerator landscapePrefab;
        
        [SerializeField] private int xResolution = 20;
        [SerializeField] private int zResolution = 20;

        [SerializeField] private float meshScale = 1;
        
        private const int MaxResolutionOfSingleChunk = 10;

        void Awake()
        {
            var chunksCountX = (int)Mathf.Ceil((float) xResolution / MaxResolutionOfSingleChunk);
            int chunkSizeX = xResolution / chunksCountX;

            var chunksCountZ = (int)Mathf.Ceil((float) zResolution / MaxResolutionOfSingleChunk);
            int chunkSizeZ = zResolution / chunksCountZ;

            var frames = new MapFrameBuilder[chunksCountX, chunksCountZ];


            var map = new Map(
                xResolution,
                zResolution,
                frames,
                chunksCountX,
                chunksCountZ);

            for (int i = 0; i < chunksCountX; i++)
            {
                for (int j = 0; j < chunksCountZ; j++)
                {
                   frames[i,j] =  CreateTerrainChunk(i,j,map, new Vector2(i * chunkSizeX * meshScale, j * chunkSizeZ * meshScale), chunkSizeX,chunkSizeZ);
                   
                }
            }
            map.BuildMountain(4,4, 2,2);
            map.BuildHollow(10,10, -2,2);
            map.CommitChanges();
        }

        private MapFrameBuilder CreateTerrainChunk(int i, int j, Map map, Vector2 location, int chunkSizeX, int chunkSizeZ)
        {
            MapGenerator chunk = Instantiate(landscapePrefab);
            chunk.Location = new Vector3(location.x, 0, location.y);
            chunk.XResolution = chunkSizeX;
            chunk.ZResolution = chunkSizeZ;
            chunk.Scale = 1;
            var frame = chunk.GenerateMap(i,j,map);

            chunk.transform.SetParent(transform);

            return frame;
        }
    }
}