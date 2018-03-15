using System;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] protected Material Material;
        [SerializeField] private Vector3 Location;
        [SerializeField] [Range(1, 255)] private int XResolution;
        [SerializeField] [Range(1, 255)] private int ZResolution;
        [SerializeField] [Range(0, 1000)] private float Scale = 1f;

        void Update()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            try
            {
                var map = new MapFrameBuilder(
                    Location,
                    XResolution,
                    ZResolution,
                    Scale,
                    Material,
                    GetComponent<MeshFilter>(),
                    GetComponent<MeshRenderer>(),
                    GetComponent<MeshCollider>()).Build();

//                map.BuildMountain(20, 10, 8, 2);
                map.BuildMountain(20, 20, 8, 2);
//                map.BuildMountain(20, 30, 5, 2);

                //map.BuildHollow(20, 10, 5, 2);


                map.CommitChanges();
            }
            catch (Exception e)
            {
                print("Map generator error: " + e.Message);
            }
        }
    }
}