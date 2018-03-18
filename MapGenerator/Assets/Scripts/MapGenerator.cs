using System;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] public Material Material;
        [SerializeField] public Vector3 Location;
        [SerializeField] [Range(1, 255)] public int XResolution;
        [SerializeField] [Range(1, 255)] public int ZResolution;
        [SerializeField] [Range(0, 1000)] public float Scale = 1f;

        public MapFrameBuilder GenerateMap(int i, int j, Map map)
        {
            try
            {
                var frame = new MapFrameBuilder(
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
                    map);
                frame.Build();
                return frame;
            }
            catch (Exception e)
            {
                print("Map generator error: " + e.Message);
                throw e;
            }
        }
    }
}