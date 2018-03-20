using System;
using Assets.Scripts.MapChanging;
using UnityEngine;

namespace Assets.Scripts.MapMeshGenerating
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class MapChunkBuilder : MonoBehaviour
    {
        [SerializeField] public Material Material;
        [SerializeField] public Vector3 Location;
        [SerializeField] [Range(1, 255)] public int XResolution;
        [SerializeField] [Range(1, 255)] public int ZResolution;
        [SerializeField] [Range(0, 1000)] public float Scale = 1f;

        public MapChunk GenerateMap(int i, int j, MapChanger mapChanger, MapChunksAccessor MapChunksAccessor)
        {
            try
            {
                var frame = new MapChunk(
                    Location,
                    XResolution,
                    ZResolution,
                    Scale,
                    Material,
                    GetComponent<MeshFilter>(),
                    GetComponent<MeshRenderer>(),
                    GetComponent<MeshCollider>(),
                    i,
                    j,
                    mapChanger,
                    MapChunksAccessor);
                frame.Build();
                return frame;
            }
            catch (Exception e)
            {
                print("MapChanger generator error: " + e.Message);
                throw e;
            }
        }
    }
}