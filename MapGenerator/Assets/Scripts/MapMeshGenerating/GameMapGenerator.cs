using System.Diagnostics;
using Assets.Scripts.MapChanging;
using Assets.Scripts.RandomMapGenerating;
using UnityEngine;

namespace Assets.Scripts.MapMeshGenerating
{
    public class GameMapGenerator : MonoBehaviour
    {

        [SerializeField] private MapChunkBuilder mapChunkPrefab;
        
        [SerializeField] private int xResolution = 20;
        [SerializeField] private int zResolution = 20;

        [SerializeField] private float meshScale = 1;
        
        private const int MaxResolutionOfSingleChunk = 25;

        void Awake()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var time1 = stopwatch.ElapsedMilliseconds;
            print("time1 " + time1);

            var chunksCountX = (int)Mathf.Ceil((float) xResolution / MaxResolutionOfSingleChunk);
            int chunkSizeX = xResolution / chunksCountX;

            var chunksCountZ = (int)Mathf.Ceil((float) zResolution / MaxResolutionOfSingleChunk);
            int chunkSizeZ = zResolution / chunksCountZ;

            var chunks = new MapChunk[chunksCountX, chunksCountZ];
            
            var MapChunksAccessor = new MapChunksAccessor();
            var map = new MapChanger(
                xResolution,
                zResolution,
                MapChunksAccessor);
            

            for (int i = 0; i < chunksCountX; i++)
            {
                for (int j = 0; j < chunksCountZ; j++)
                {
                   chunks[i,j] =  CreateTerrainChunk(i,j,map, MapChunksAccessor, new Vector2(i * chunkSizeX * meshScale, j * chunkSizeZ * meshScale), chunkSizeX,chunkSizeZ);
                   
                }
            }
           
            MapChunksAccessor.Recalculate(map);

            var randomMapGenerator = new RandomMapGenerator();
            randomMapGenerator.GenerateFor(map);
            
            foreach (var mapChunk in chunks)
            {
                mapChunk.CommitChanges();
            }

            var time2 = stopwatch.ElapsedMilliseconds;
            print("time2 " + time2);
        }

        private MapChunk CreateTerrainChunk(int i, int j, MapChanger mapChanger, MapChunksAccessor MapChunksAccessor, Vector2 location, int chunkSizeX, int chunkSizeZ)
        {
            MapChunkBuilder chunk = Instantiate(mapChunkPrefab);
            chunk.Location = new Vector3(location.x, 0, location.y);
            chunk.XResolution = chunkSizeX;
            chunk.ZResolution = chunkSizeZ;
            chunk.Scale = 1;
            var frame = chunk.GenerateMap(i,j,mapChanger, MapChunksAccessor);

            chunk.transform.SetParent(transform);

            return frame;
        }
    }
}