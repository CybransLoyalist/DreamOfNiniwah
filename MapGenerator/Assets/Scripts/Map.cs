using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Map : IMap
    {
        private readonly List<Vector3> _vertices;
        private readonly List<Color32> _vertexColors;
        private readonly int _numberOfVertices;
        private readonly List<int>[,] _verticesLocations;
        private readonly float _scale;
        private readonly int _zResolution;
        private readonly int _xResolution;
        private readonly Action _commitChangesCallback;

        public Map(
            int xResolution,
            int zResolution, 
            float scale, 
            List<int>[,] verticesLocations,
            List<Vector3> vertices,
            List<Color32> vertexColors, 
            int numberOfVertices, 
            Action commitChangesCallback)
        {
            _xResolution = xResolution;
            _zResolution = zResolution;
            _scale = scale;
            _verticesLocations = verticesLocations;
            _vertices = vertices;
            _vertexColors = vertexColors;
            _numberOfVertices = numberOfVertices;
            _commitChangesCallback = commitChangesCallback;
        }


        private void RaiseTile(int x, int y, float height)
        {
            for (var i = x * 2; i <= x * 2 + 2; ++i)
            {
                for (var j = y * 2; j <= y * 2 + 2; ++j)
                {
                    foreach (var index in _verticesLocations[i, j])
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
                if (_scale * height > _vertices[index].y)
                {
                    _vertices[index] = new Vector3(_vertices[index].x, _scale * height, _vertices[index].z);
                }
            }
        }

        private void ColorTile(int x, int y, Color color)
        {
            for (var i = x * 2; i <= x * 2 + 2; ++i)
            {
                for (var j = y * 2; j <= y * 2 + 2; ++j)
                {
                    foreach (var index in _verticesLocations[i, j])
                    {
                        ColorTile(index, color);
                    }
                }
            }
            if (x * 2 + 1 < _xResolution * 2 + 1 && y * 2 - 1 < _zResolution * 2 + 1)
            {
                foreach (var index in _verticesLocations[x * 2 + 1, y * 2 - 1])
                {
                    ColorTile(index, color);
                }
            }
            if (x * 2 + 1 < _xResolution * 2 + 1 && y * 2 + 3 < _zResolution * 2 + 1)
            {
                foreach (var index in _verticesLocations[x * 2 + 1, y * 2 + 3])
                {
                    ColorTile(index, color);
                }
            }
            if (x * 2 + 3 < _xResolution * 2 + 1 && y * 2 + 1 < _zResolution * 2 + 1)
            {
                foreach (var index in _verticesLocations[x * 2 + 3, y * 2 + 1])
                {
                    ColorTile(index, color);
                }
            }
            if (x * 2 - 1 < _xResolution * 2 + 1 && y * 2 + 1 < _zResolution * 2 + 1)
            {
                foreach (var index in _verticesLocations[x * 2 - 1, y * 2 + 1])
                {
                    ColorTile(index, color);
                }
            }
        }

        private void ColorTileExact(int x, int y, Color color)
        {
            foreach (var index in _verticesLocations[x * 2 + 1, y * 2 + 1])
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

        public void BuildMountain(int x, int y, int peakHeigh, int ringWidth)
        {
            var tileHeights = MountainBuilder.BuildMountain(x, y, peakHeigh, ringWidth);
            foreach (var tileHeight in tileHeights)
            {
                RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
            }
        }


        public void SetYPositionOfMiddleVertices()
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

        public void CommitChanges()
        {
            SetYPositionOfMiddleVertices();
            _commitChangesCallback();
        }
    }
}