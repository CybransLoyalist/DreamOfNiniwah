using UnityEngine;

namespace Assets.Scripts
{
    public class ProceduralInfiniteLandscapeGenerator : MonoBehaviour
    {

        [SerializeField] private MapGenerator landscapePrefab;

        [SerializeField] private Material material;
        [SerializeField] private int xResolution = 20;
        [SerializeField] private int zResolution = 20;

        [SerializeField] private float meshScale = 1;
        [SerializeField] private float yScale = 1;

        [SerializeField, Range(1, 8)] private int octaves = 1;
        [SerializeField] private float lacunarity = 2;

        [SerializeField, Range(0, 1)]
        private float gain = 0.5f
            ; //needs to be between 0 and 1 so that each octave contributes less to the final shape.

        [SerializeField] private float perlinScale = 1;
        [SerializeField] private float uvScale = 1;
        private const int MaxResolutionOfSingleChunk = 2;

        void Awake()
        {
            var chunksCountX = (int)Mathf.Ceil((float) xResolution / MaxResolutionOfSingleChunk);
            int chunkSizeX = xResolution / chunksCountX;

            var chunksCountZ = (int)Mathf.Ceil((float) zResolution / MaxResolutionOfSingleChunk);
            int chunkSizeZ = zResolution / chunksCountZ;

            var frames = new MapFrameBuilder[chunksCountX, chunksCountZ];

            for (int i = 0; i < chunksCountX; i++)
            {
                for (int j = 0; j < chunksCountZ; j++)
                {
                   frames[i,j] =  CreateTerrainChunk(new Vector2(i * chunkSizeX * meshScale, j * chunkSizeZ * meshScale), chunkSizeX,chunkSizeZ);
                   
                }
            }
            
            var map = new Map2(
                xResolution,
                zResolution,
                meshScale,
                frames,
                chunksCountX,
                chunksCountZ,
                chunkSizeX,
                chunkSizeZ);


//            map.ColorTileExact(3,3, Color.cyan);
//            map.RaiseTile(3,3, 1f);
//            map.LowerTile(0,0, -1f);

            map.ColorTile(3,3, Color.magenta);
            map.CommitChanges();
        }

        private MapFrameBuilder CreateTerrainChunk(Vector2 location, int chunkSizeX, int chunkSizeZ)
        {
            MapGenerator chunk = Instantiate(landscapePrefab);
            chunk.Location = new Vector3(location.x, 0, location.y);
            chunk.XResolution = chunkSizeX;
            chunk.ZResolution = chunkSizeZ;
            chunk.Scale = 1;
            var frame = chunk.GenerateMap();

            chunk.transform.SetParent(transform);

            return frame;
        }
    }
}