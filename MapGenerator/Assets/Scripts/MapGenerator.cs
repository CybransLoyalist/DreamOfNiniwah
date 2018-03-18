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

//        void Update()
//        {
//            GenerateMap();
//        }

        public MapFrameBuilder GenerateMap(int i, int j, Map3 map)
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
                    i,j,
                    map);
                frame.Build();
                return frame;
                // map.BuildMountain(20, 30, 5, 2);

                //  map.CommitChanges();
            }
            catch (Exception e)
            {
                print("Map generator error: " + e.Message);
                throw e;
            }
        }
    }
}