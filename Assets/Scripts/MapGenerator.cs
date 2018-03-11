using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MapGenerator : AbstractMeshGenerator
    {
        protected override string Name
        {
            get { return "Map"; }
        }
        
        [SerializeField] private Vector3 Location;
        [SerializeField] [Range(1, 255)] private int XResolution;
        [SerializeField] [Range(1, 255)] private int ZResolution;
        [SerializeField] [Range(0, 1000)] private float Scale = 1f;

        private int TilesCount;
        private int VerticesXCount;
        private int VerticesZCount;
        private List<int>[,] VerticesLocations;

        private const int VerticesPerTile = 5;
        private const int TrianglesPerTile = 4;
        private const int TriangleCornersInTriangle = 3;
        private readonly Color _baseColor = Color.yellow;

        protected override void SetMeshNumbers()
        {
            TilesCount = XResolution * ZResolution;
            VerticesXCount = XResolution * 2 + 1;
            VerticesZCount = ZResolution * 2 + 1;

            _numberOfVertices = TilesCount * VerticesPerTile;
            _numberOfTrianglesCorners = TilesCount * TrianglesPerTile * TriangleCornersInTriangle;
            InitializeVerticesLocations();
        }

        private void InitializeVerticesLocations()
        {
            VerticesLocations = new List<int>[VerticesXCount, VerticesZCount];
            for (int z = 0; z < VerticesZCount; z++)
            {
                for (int x = 0; x < VerticesXCount; x++)
                {
                    VerticesLocations[x, z] = new List<int>();
                }
            }
        }

        protected override void SetVertices()
        {
            int counter = 0;
            for (int z = 0; z < ZResolution; z++)
            {
                for (int x = 0; x < XResolution; x++)

                {
                    var vertex00 = new Vector3(Location.x + (float) (x) * Scale, Location.y,
                        Location.z + (float) (z) * Scale);
                    var vertex01 = new Vector3(Location.x + (float) (x) * Scale, Location.y,
                        Location.z + (float) (z + 1) * Scale);
                    var vertex11 = new Vector3(Location.x + (float) (x + 1) * Scale, Location.y,
                        Location.z + (float) (z + 1) * Scale);
                    var vertex10 = new Vector3(Location.x + (float) (x + 1) * Scale, Location.y,
                        Location.z + (float) (z) * Scale);
                    var vertexMiddle = new Vector3(Location.x + ((float) (x) + 0.5f) * Scale, Location.y,
                        Location.z + ((float) (z) + 0.5f) * Scale);


                    VerticesLocations[x * 2, z * 2].Add(counter);
                    VerticesLocations[x * 2, z * 2 + 2].Add(counter + 1);
                    VerticesLocations[x * 2 + 2, z * 2 + 2].Add(counter + 2);
                    VerticesLocations[x * 2 + 2, z * 2].Add(counter + 3);
                    VerticesLocations[x * 2 + 1, z * 2 + 1].Add(counter + 4);
                    counter += 5;


                    _vertices.Add(vertex00);
                    _vertices.Add(vertex01);
                    _vertices.Add(vertex11);
                    _vertices.Add(vertex10);
                    _vertices.Add(vertexMiddle);
                }
            }


            RaiseTile(2, 2, 1f);
            RaiseTile(3, 1, 1f);
            RaiseTile(4, 2, 1f);

            RaiseTile(1, 3, 1f);
            RaiseTile(5, 3, 1f);

            RaiseTile(2, 4, 1f);
            RaiseTile(4, 4, 1f);
            RaiseTile(3, 5, 1f);

            RaiseTile(3, 4, 2f);
            RaiseTile(3, 2, 2f);
            RaiseTile(2, 3, 2f);
            RaiseTile(4, 3, 2f);
            RaiseTile(3, 3, 2f);

            ColorTile(3, 3, Color.green);
            ColorTile(6, 6, Color.green);

            for (int i = 0; i < _numberOfVertices; i += 5)
            {
                var middleVertice = i + 4;

                var totalHeight = 0f;
                for (int j = i; j < i + 4; j++)
                {
                    totalHeight += _vertices[j].y;
                }

                _vertices[middleVertice] = new Vector3(_vertices[middleVertice].x, totalHeight / 4f,
                    _vertices[middleVertice].z);

            }



        }

        private void RaiseTile(int x, int y, float height)
        {
            for (int i = x * 2; i <= x * 2 + 2; ++i)
            {
                for (int j = y * 2; j <= y * 2 + 2; ++j)
                {
                    foreach (var index in VerticesLocations[i, j])
                    {
                        RaiseVertex(index, height);
                    }
                }
            }
        }

        private void RaiseVertex(int index, float height)
        {
            if (_vertices.Count > index)
            {
                if (Scale * height > _vertices[index].y)
                {
                    _vertices[index] = new Vector3(_vertices[index].x, Scale * height, _vertices[index].z);
                }
            }
        }

        private void ColorTile(int x, int y, Color color)
        {
            for (int i = x * 2; i <= x * 2 + 2; ++i)
            {
                for (int j = y * 2; j <= y * 2 + 2; ++j)
                {
                    foreach (var index in VerticesLocations[i, j])
                    {
                        ColorTile(index, color);
                    }
                }
            }
            if (x * 2 + 1 < XResolution * 2 + 1 && y * 2 - 1 < ZResolution * 2 + 1)
            {
                foreach (var index in VerticesLocations[x * 2 + 1, y * 2 - 1])
                {
                    ColorTile(index, color);
                }
            }
            if (x * 2 + 1 < XResolution * 2 + 1 && y * 2 + 3 < ZResolution * 2 + 1)
            {
                foreach (var index in VerticesLocations[x * 2 + 1, y * 2 + 3])
                {
                    ColorTile(index, color);
                }
            }
            if (x * 2 + 3 < XResolution * 2 + 1 && y * 2 + 1 < ZResolution * 2 + 1)
            {
                foreach (var index in VerticesLocations[x * 2 + 3, y * 2 + 1])
                {
                    ColorTile(index, color);
                }
            }
            if (x * 2 - 1 < XResolution * 2 + 1 && y * 2 + 1 < ZResolution * 2 + 1)
            {
                foreach (var index in VerticesLocations[x * 2 - 1, y * 2 + 1])
                {
                    ColorTile(index, color);
                }
            }
        }

        private void ColorTile(int index, Color color)
        {
            if (_vertexColors.Count == 0)
            {
                for (int i = 0; i < _numberOfVertices; i++)
                {
                    _vertexColors.Add(_baseColor);
                }
            }
            if (_vertices.Count > index)
            {
                _vertexColors[index] = color;
//                if (Scale * height > _vertices[index].y)
//                {
//                    _vertices[index] = new Vector3(_vertices[index].x, Scale * height, _vertices[index].z);
//                }
            }
        }

        protected override void SetTriangles()
        {
            var trianglesCount = 0;
            for (int z = 0; z < ZResolution; z++)
            {
                for (int x = 0; x < XResolution; x++)
                {
                    _triangles.Add(trianglesCount);
                    _triangles.Add(trianglesCount + 1);
                    _triangles.Add(trianglesCount + 4);

                    _triangles.Add(trianglesCount + 1);
                    _triangles.Add(trianglesCount + 2);
                    _triangles.Add(trianglesCount + 4);

                    _triangles.Add(trianglesCount + 2);
                    _triangles.Add(trianglesCount + 3);
                    _triangles.Add(trianglesCount + 4);

                    _triangles.Add(trianglesCount + 0);
                    _triangles.Add(trianglesCount + 4);
                    _triangles.Add(trianglesCount + 3);
                    trianglesCount += 5;
                }
            }
        }

        //        protected override void SetUVs()
        //        {
        //            _uvs.AddRange(flexibleUvs);
        //        }

//        protected override void SetUVs()
//        {
//            for (int z = 0; z <= ZResolution * 2 + 1; z++)
//            {
//                for (int x = 0; x <= XResolution * 2 + 1; x++)
//                {
//                    _uvs.Add(new Vector2(x / (1f * XResolution), z / (1f * ZResolution)));
//                }
//            }
//        }

        protected override void SetVertexColours()
        {
//            for (int i = 0; i < 30; i++)
//            {
//                _vertexColors.Add(Color.yellow);
//            }
//            for (int i = 30; i < 60; i++)
//            {
//                _vertexColors.Add(Color.blue);
//            }
//            for (int i = 60; i < _numberOfVertices; i++)
//            {
//                _vertexColors.Add(Color.red);
//            }
        }
    }
}