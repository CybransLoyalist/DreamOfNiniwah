using System;
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
            for (var z = 0; z < VerticesZCount; z++)
            {
                for (var x = 0; x < VerticesXCount; x++)
                {
                    VerticesLocations[x, z] = new List<int>();
                }
            }
        }

        protected override void SetVertices()
        {
            var counter = 0;
            for (var z = 0; z < ZResolution; z++)
            {
                for (var x = 0; x < XResolution; x++)

                {
                    var vertex00 = new Vector3(Location.x + x * Scale, Location.y, Location.z + z * Scale);
                    var vertex01 = new Vector3(Location.x + x * Scale, Location.y, Location.z + (z + 1) * Scale);
                    var vertex11 = new Vector3(Location.x + (x + 1) * Scale, Location.y, Location.z + (z + 1) * Scale);
                    var vertex10 = new Vector3(Location.x + (x + 1) * Scale, Location.y, Location.z + z * Scale);
                    var vertexMiddle = new Vector3(Location.x + (x + 0.5f) * Scale, Location.y,
                        Location.z + (z + 0.5f) * Scale);


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

            if (_vertexColors.Count == 0)
            {
                for (var i = 0; i < _numberOfVertices; i++)
                {
                    _vertexColors.Add(_baseColor);
                }
            }

            BuildMountain(20, 10, 5, 2);
            BuildMountain(20, 20, 8, 2);
            BuildMountain(20, 30, 5, 2);


            SetYPositionOfMiddleVertices();
        }

        private void BuildMountain(int x, int y, int peakHeigh, int ringWidth)
        {
            var tileHeights = MountainBuilder.BuildMountain(x, y, peakHeigh, ringWidth);
            foreach (var tileHeight in tileHeights)
            {
                RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
            }
        }

        private void SetYPositionOfMiddleVertices()
        {
            for (var i = 0; i < _numberOfVertices; i += 5)
            {
                var middleVertice = i + 4;

                var totalHeight = 0f;
                for (var j = i; j < i + 4; j++)
                {
                    totalHeight += _vertices[j].y;
                }

                _vertices[middleVertice] = new Vector3(_vertices[middleVertice].x, totalHeight / 4f,
                    _vertices[middleVertice].z);
            }
        }

        private void RaiseTile(int x, int y, float height)
        {
            for (var i = x * 2; i <= x * 2 + 2; ++i)
            {
                for (var j = y * 2; j <= y * 2 + 2; ++j)
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
            for (var i = x * 2; i <= x * 2 + 2; ++i)
            {
                for (var j = y * 2; j <= y * 2 + 2; ++j)
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

        private void ColorTileExact(int x, int y, Color color)
        {
            foreach (var index in VerticesLocations[x * 2 + 1, y * 2 + 1])
            {
                ColorTile(index, color);
            }
        }

        private void ColorTile(int index, Color color)
        {
            if (_vertices.Count > index)
            {
                _vertexColors[index] = color;
            }
        }

        protected override void SetTriangles()
        {
            var trianglesCount = 0;
            for (var z = 0; z < ZResolution; z++)
            {
                for (var x = 0; x < XResolution; x++)
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


        protected override void SetVertexColours()
        {
        }
    }
}