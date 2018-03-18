using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IMapChunk
    {
        List<Color32> _vertexColors { get; set; }
        List<Vector3> _vertices { get; set; }
        void CommitChanges();
        int X { get; set; }
        int Z { get; set; }
    }


    public class Vertex
    {
        public Vector3 Vector3
        {
            get { return Chunk._vertices[Index]; }
        }

        public IMapChunk Chunk { get; set; }
        public int Index { get; set; }
        public bool IsMiddle { get; set; }
    }

    public class Tile
    {
        public List<Vertex> Vertices { get; set; }
    }

    public class Map3 : IMap
    {
        public IMapChunk[,] _chunks;
        private readonly int _zResolution;
        private readonly int _xResolution;
        private int _chunksCountX;
        private int _chunksCountZ;
        public Dictionary<Vector2Int, Tile> Tiles;

        public Map3(
            int xResolution,
            int zResolution,
            IMapChunk[,] chunks,
            int chunksCountX,
            int chunksCountZ)
        {
            _xResolution = xResolution;
            _zResolution = zResolution;
            _chunksCountX = chunksCountX;
            _chunksCountZ = chunksCountZ;
            _chunks = chunks;

            Tiles = new Dictionary<Vector2Int, Tile>();
            for (int x = 0; x < xResolution; x++)
            {
                for (int z = 0; z < _zResolution; z++)
                {
                    Tiles[new Vector2Int(x, z)] = new Tile() {Vertices = new List<Vertex>()};
                }
            }
//            _chunks = new IMapChunk[chunksCountX, chunksCountZ];
//            for (int x = 0; x < chunksCountX; x++)
//            {
//                for (int z = 0; z < chunksCountZ; z++)
//                {
//                    _chunks[x,z] = new MapChunk();
//                }
//            }
        }

        public void CommitChanges()
        {
            for (int i = 0; i < _chunksCountX; i++)
            {
                for (int j = 0; j < _chunksCountZ; j++)
                {
                    _chunks[i, j].CommitChanges();
                }
            }
        }

        public void ColorTileExact(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                tileVertex.Chunk._vertexColors[tileVertex.Index] = color;
            }
            
        }
        public void RaiseTile(int x, int y, float height)
        {

            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                RaiseVertex(tileVertex, height);
            }
            var vectors = tile.Vertices.Select(a => a.Vector3);
            var neighbours = GetNeighbours(x, y);
            foreach (var neighbour in neighbours)
            {
                var matchingVertices = Tiles[new Vector2Int(neighbour.x, neighbour.y)].Vertices
                    .Where(a => vectors.Any(v => v.x == a.Vector3.x && v.z == a.Vector3.z)).ToList();
                foreach (var matchingVertex in matchingVertices)
                {
                    RaiseVertex(matchingVertex, height);
                    //  matchingVertex.Chunk._vertexColors[matchingVertex.Index] = color;
                }
                
            }
        }

        private void RaiseVertex(Vertex vertex,  float height)
        {

            vertex.Chunk._vertices[vertex.Index] = new Vector3(vertex.Chunk._vertices[vertex.Index].x, height, vertex.Chunk._vertices[vertex.Index].z);
        }

        public void LowerTile(int x, int y, float height)
        {

            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                LowerVertex(tileVertex, height);
            }
            var vectors = tile.Vertices.Select(a => a.Vector3);
            var neighbours = GetNeighbours(x, y);
            foreach (var neighbour in neighbours)
            {
                var matchingVertices = Tiles[new Vector2Int(neighbour.x, neighbour.y)].Vertices
                    .Where(a => vectors.Any(v => v.x == a.Vector3.x && v.z == a.Vector3.z)).ToList();
                foreach (var matchingVertex in matchingVertices)
                {
                    LowerVertex(matchingVertex, height);
                    //  matchingVertex.Chunk._vertexColors[matchingVertex.Index] = color;
                }
                
            }
        }

        private void LowerVertex(Vertex vertex,  float height)
        {

            vertex.Chunk._vertices[vertex.Index] = new Vector3(vertex.Chunk._vertices[vertex.Index].x, height, vertex.Chunk._vertices[vertex.Index].z);
        }
     
        

        public void ColorTile(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];
            foreach (var tileVertex in tile.Vertices)
            {
                tileVertex.Chunk._vertexColors[tileVertex.Index] = color;
            }

            var vectors = tile.Vertices.Select(a => a.Vector3);
            var neighbours = GetNeighbours(x, y);
            foreach (var neighbour in neighbours)
            {
                var matchingVertices = Tiles[new Vector2Int(neighbour.x, neighbour.y)].Vertices
                    .Where(a => vectors.Contains(a.Vector3));
                foreach (var matchingVertex in matchingVertices)
                {
                    matchingVertex.Chunk._vertexColors[matchingVertex.Index] = color;
                }
                if (neighbour.x == x || neighbour.y == y)
                {
                    ColorMiddleVertexOfTile(neighbour.x, neighbour.y, color);
                }
            }
        }

        
                public void BuildMountain(int x, int y, int peakHeigh, int ringWidth)
                {
                    if (peakHeigh <= 0)
                    {
                        Debug.LogWarning("Building mountain with 0 or lower peak height");
                        return;
                    }
                    var tileHeights = MountainBuilder.BuildMountain(x, y, peakHeigh, ringWidth, _xResolution, _zResolution);
                    foreach (var tileHeight in tileHeights)
                    {
                        RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
                    }
                }

        public void ColorMiddleVertexOfTile(int x, int y, Color color)
        {
            var tile = Tiles[new Vector2Int(x, y)];

            var middleVertes = tile.Vertices.First(a => a.IsMiddle);
            middleVertes.Chunk._vertexColors[middleVertes.Index] = color;
        }

        private List<Vector2Int> GetNeighbours(int x, int y)
        {
            var result = new List<Vector2Int>();
            if (x - 1 >= 0)
            {
                result.Add(new Vector2Int(x - 1, y));
            }
            if (x + 1 < _xResolution)
            {
                result.Add(new Vector2Int(x + 1, y));
            }
            if (y - 1 >= 0)
            {
                result.Add(new Vector2Int(x, y - 1));
            }
            if (y + 1 < _zResolution)
            {
                result.Add(new Vector2Int(x, y + 1));
            }

            if (x - 1 >= 0 && y - 1 >= 0)
            {
                result.Add(new Vector2Int(x - 1, y - 1));
            }
            if (x + 1 < _xResolution && y + 1 < _zResolution)
            {
                result.Add(new Vector2Int(x + 1, y + 1));
            }
            if (x + 1 < _xResolution && y - 1 >= 0)
            {
                result.Add(new Vector2Int(x + 1, y - 1));
            }
            if (x - 1 >= 0 && y + 1 < _zResolution)
            {
                result.Add(new Vector2Int(x - 1, y + 1));
            }
            return result;
        }

        public List<IMapChunk> GetChunksOfTile(int x, int y)
        {
            throw new NotImplementedException();
        }

        private bool IsValidPointOnMap(int x, int y)
        {
            return MapOperationValidator.IsValidPointOnMap(x, y, _xResolution, _zResolution);
        }
    }

//    public class Map2 : MonoBehaviour, IMap
//    {
////        private readonly List<Vector3> _vertices;
////        private readonly List<Color32> _vertexColors;
////        private readonly int _numberOfVertices;
////        private readonly List<int>[,] _verticesLocations;
//        private readonly float _scale;
//
//        private readonly int _zResolution;
//
//        private readonly int _xResolution;
//
////        private readonly Action _commitChangesCallback;
//        private MapFrameBuilder[,] _chunks;
//
//        private int _chunksCountX;
//        private int _chunksCountZ;
//        private int _chunkSizeX;
//        private int _chunkSizeZ;
//
//        public Map2(
//            int xResolution,
//            int zResolution,
//            float scale,
//            MapFrameBuilder[,] chunks,
//            int chunksCountX,
//            int chunksCountZ,
//            int chunkSizeX,
//            int chunkSizeZ)
//        {
//            _xResolution = xResolution;
//            _zResolution = zResolution;
//            _scale = scale;
//            _chunks = chunks;
//            _chunksCountX = chunksCountX;
//            _chunksCountZ = chunksCountZ;
//            _chunkSizeX = chunkSizeX;
//            _chunkSizeZ = chunkSizeZ;
////            _verticesLocations = verticesLocations;
////            _vertices = vertices;
////            _vertexColors = vertexColors;
////            _numberOfVertices = numberOfVertices;
////            _commitChangesCallback = commitChangesCallback;
//        }
//
//
//        public void RaiseTile(int x, int y, float height)
//        {
//            var vertices = GetAllVerticesOfTile(x, y);
//            foreach (var vertex in vertices)
//            {
//                var chunkX = x / _chunksCountX;
//                var chunkZ = y / _chunksCountZ;
//                var chunk = _chunks[chunkX, chunkZ];
//
//                RaiseVertex(chunk, vertex, height);
//            }
//        }
//
////
//        public void LowerTile(int x, int y, float height)
//        {
//            var vertices = GetAllVerticesOfTile(x, y);
//            foreach (var vertex in vertices)
//            {
//                var chunkX = x / _chunksCountX;
//                var chunkZ = y / _chunksCountZ;
//                var chunk = _chunks[chunkX, chunkZ];
//
//                LowerVertex(chunk, vertex, height);
//            }
//        }
//
//        private void RaiseVertex(MapFrameBuilder chunk, int index, float height)
//        {
//            if (chunk._vertices.Count > index)
//            {
//                if (_scale * height > chunk._vertices[index].y)
//                {
//                    chunk._vertices[index] = new Vector3(chunk._vertices[index].x, _scale * height,
//                        chunk._vertices[index].z);
//                }
//            }
//        }
//
//        private void LowerVertex(MapFrameBuilder chunk, int index, float height)
//        {
//            if (chunk._vertices.Count > index)
//            {
//                if (_scale * height < chunk._vertices[index].y)
//                {
//                    chunk._vertices[index] = new Vector3(chunk._vertices[index].x, _scale * height,
//                        chunk._vertices[index].z);
//                }
//            }
//        }
//
//        public void ColorTile(int x, int y, Color color)
//        {
//            if (IsValidPointOnMap(x, y))
//            {
//                var tileVertices = GetAllVerticesOfTile(x, y);
//
//                var chunkX = x / _chunksCountX;
//                var chunkZ = y / _chunksCountZ;
//                var chunk = _chunks[chunkX, chunkZ];
//                foreach (var index in tileVertices)
//                {
//                    ColorTile(chunk, index, color);
//                }
//
////                var neighbours = GetNeighbours(x, y);
////                foreach (var neighbour in neighbours)
////                {
////                    var chunkX = neighbour.x / _chunksCountX;
////                    var chunkZ = neighbour.y / _chunksCountZ;
////                    var chunk = _chunks[chunkX, chunkZ];
////
////                    var middleVertex = GetMiddleVertexOfTile(neighbour.x, neighbour.y);
////                    ColorTile(chunk, middleVertex, color);
////                }
//            }
//        }
//
//        public void ColorTileExact(int x, int y, Color color)
//        {
//            if (IsValidPointOnMap(x, y))
//            {
//                var chunkX = x / _chunksCountX;
//                var chunkZ = y / _chunksCountZ;
//                var chunk = _chunks[chunkX, chunkZ];
//
//                var middleVertex = GetMiddleVertexOfTile(x, y);
//                ColorTile(chunk, middleVertex, color);
//            }
//        }
//
//        private List<int> GetAllVerticesOfTile(int x, int y)
//        {
//            var result = new List<int>();
//
//            var chunkX = x / _chunksCountX;
//            var chunkZ = y / _chunksCountZ;
//            var chunk = _chunks[chunkX, chunkZ];
//            var xAtChunk = x % _chunksCountX;
//            var zAtChunk = y % _chunksCountZ;
//
//            for (var i = xAtChunk * 2; i <= xAtChunk * 2 + 2; ++i)
//            {
//                for (var j = zAtChunk * 2; j <= zAtChunk * 2 + 2; ++j)
//                {
//                    var verticesLocations = chunk.VerticesLocations[i, j];
//                    foreach (var index in verticesLocations)
//                    {
//                        result.Add(index);
//                    }
//                }
//            }
//
//            return result;
//        }
//
//        private List<Vector2Int> GetNeighbours(int x, int y)
//        {
//            var result = new List<Vector2Int>();
//            if (x - 1 >= 0)
//            {
//                result.Add(new Vector2Int(x - 1, y));
//            }
//            if (x + 1 < _xResolution)
//            {
//                result.Add(new Vector2Int(x + 1, y));
//            }
//            if (y - 1 >= 0)
//            {
//                result.Add(new Vector2Int(x, y - 1));
//            }
//            if (y + 1 < _zResolution)
//            {
//                result.Add(new Vector2Int(x, y + 1));
//            }
//            return result;
//        }
//
////
//        private int GetMiddleVertexOfTile(int x, int y)
//        {
//            var chunkX = x / _chunksCountX;
//            var chunkZ = y / _chunksCountZ;
//            var chunk = _chunks[chunkX, chunkZ];
//            var xAtChunk = x % _chunksCountX;
//            var zAtChunk = y % _chunksCountZ;
//            return chunk.VerticesLocations[xAtChunk * 2 + 1, zAtChunk * 2 + 1].First();
//        }
//
////
//        private void ColorTile(MapFrameBuilder chunk, int index, Color color)
//        {
//            if (chunk._vertices.Count > index)
//            {
//                chunk._vertexColors[index] = color;
//            }
//        }
//
////
////        public void BuildMountain(int x, int y, int peakHeigh, int ringWidth)
////        {
////            if (peakHeigh <= 0)
////            {
////                Debug.LogWarning("Building mountain with 0 or lower peak height");
////                return;
////            }
////            var tileHeights = MountainBuilder.BuildMountain(x, y, peakHeigh, ringWidth, _xResolution, _zResolution);
////            foreach (var tileHeight in tileHeights)
////            {
////                RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
////            }
////        }
////
////        public void BuildHollow(int x, int y, int bottomDepth, int ringWidth)
////        {
////            if (bottomDepth >= 0)
////            {
////                Debug.LogWarning("Building hollow with 0 or higher bottom depth.");
////                return;
////            }
////
////            var tileHeights = MountainBuilder.BuildHollow(x, y, bottomDepth, ringWidth, _xResolution, _zResolution);
////            foreach (var tileHeight in tileHeights)
////            {
////                LowerTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
////            }
////        }
////
////        public void SetYPositionOfMiddleVertices()
////        {
////            for (var i = 0; i < _numberOfVertices; i += 5)
////            {
////                var middleVertice = i + 4;
////
////                var totalHeight = 0f;
////                for (var j = i; j < i + 4; j++)
////                {
////                    totalHeight += _vertices[j].y;
////                }
////
////                _vertices[middleVertice] = new Vector3(_vertices[middleVertice].x, totalHeight / 4f,
////                    _vertices[middleVertice].z);
////            }
////        }
////
//        public void CommitChanges()
//        {
//            for (int i = 0; i < _chunksCountX; i++)
//            {
//                for (int j = 0; j < _chunksCountZ; j++)
//                {
//                    _chunks[i, j].CommitChanges();
//                }
//            }
//        }
//
//        private bool IsValidPointOnMap(int x, int y)
//        {
//            return MapOperationValidator.IsValidPointOnMap(x, y, _xResolution, _zResolution);
//        }
//    }


//    public class Map : IMap
//    {
//        private readonly List<Vector3> _vertices;
//        private readonly List<Color32> _vertexColors;
//        private readonly int _numberOfVertices;
//        private readonly List<int>[,] _verticesLocations;
//        private readonly float _scale;
//        private readonly int _zResolution;
//        private readonly int _xResolution;
//        private readonly Action _commitChangesCallback;
//
//        public Map(
//            int xResolution,
//            int zResolution,
//            float scale,
//            List<int>[,] verticesLocations,
//            List<Vector3> vertices,
//            List<Color32> vertexColors,
//            int numberOfVertices,
//            Action commitChangesCallback)
//        {
//            _xResolution = xResolution;
//            _zResolution = zResolution;
//            _scale = scale;
//            _verticesLocations = verticesLocations;
//            _vertices = vertices;
//            _vertexColors = vertexColors;
//            _numberOfVertices = numberOfVertices;
//            _commitChangesCallback = commitChangesCallback;
//        }
//
//        private void RaiseTile(int x, int y, float height)
//        {
//            var vertices = GetAllVerticesOfTile(x, y);
//            foreach (var vertex in vertices)
//            {
//                RaiseVertex(vertex, height);
//            }
//        }
//
//        private void LowerTile(int x, int y, float height)
//        {
//            var vertices = GetAllVerticesOfTile(x, y);
//            foreach (var vertex in vertices)
//            {
//                LowerVertex(vertex, height);
//            }
//        }
//
//        private void RaiseVertex(int index, float height)
//        {
//            if (_vertices.Count > index)
//            {
//                if (_scale * height > _vertices[index].y)
//                {
//                    _vertices[index] = new Vector3(_vertices[index].x, _scale * height, _vertices[index].z);
//                }
//            }
//        }
//
//        private void LowerVertex(int index, float height)
//        {
//            if (_vertices.Count > index)
//            {
//                if (_scale * height < _vertices[index].y)
//                {
//                    _vertices[index] = new Vector3(_vertices[index].x, _scale * height, _vertices[index].z);
//                }
//            }
//        }
//
//        public void ColorTile(int x, int y, Color color)
//        {
//            if (IsValidPointOnMap(x, y))
//            {
//                var tileVertices = GetAllVerticesOfTile(x, y);
//                foreach (var index in tileVertices)
//                {
//                    ColorTile(index, color);
//                }
//
//                var neighbours = GetNeighbours(x, y);
//                foreach (var neighbour in neighbours)
//                {
//                    var middleVertex = GetMiddleVertexOfTile(neighbour.x, neighbour.y);
//                    ColorTile(middleVertex, color);
//                }
//            }
//        }
//
//        public void ColorTileExact(int x, int y, Color color)
//        {
//            if (IsValidPointOnMap(x, y))
//            {
//                var middleVertex = GetMiddleVertexOfTile(x, y);
//                ColorTile(middleVertex, color);
//            }
//        }
//
//        private List<int> GetAllVerticesOfTile(int x, int y)
//        {
//            var result = new List<int>();
//            for (var i = x * 2; i <= x * 2 + 2; ++i)
//            {
//                for (var j = y * 2; j <= y * 2 + 2; ++j)
//                {
//                    foreach (var index in _verticesLocations[i, j])
//                    {
//                        result.Add(index);
//                    }
//                }
//            }
//
//            return result;
//        }
//
//        private List<Vector2Int> GetNeighbours(int x, int y)
//        {
//            var result = new List<Vector2Int>();
//            if (x - 1 >= 0)
//            {
//                result.Add(new Vector2Int(x-1, y));
//            }
//            if (x + 1 < _xResolution)
//            {
//                result.Add(new Vector2Int(x+1, y));
//            }
//            if (y - 1 >= 0)
//            {
//                result.Add(new Vector2Int(x, y - 1));
//            }
//            if (y + 1 < _zResolution)
//            {
//                result.Add(new Vector2Int(x, y + 1));
//            }
//            return result;
//        }
//
//        private int GetMiddleVertexOfTile(int x, int y)
//        {
//            return _verticesLocations[x * 2 + 1, y * 2 + 1].First();
//        }
//
//        private void ColorTile(int index, Color color)
//        {
//            if (_vertices.Count > index)
//            {
//                _vertexColors[index] = color;
//            }
//        }
//
//        public void BuildMountain(int x, int y, int peakHeigh, int ringWidth)
//        {
//            if (peakHeigh <= 0)
//            {
//                Debug.LogWarning("Building mountain with 0 or lower peak height");
//                return;
//            }
//            var tileHeights = MountainBuilder.BuildMountain(x, y, peakHeigh, ringWidth, _xResolution, _zResolution);
//            foreach (var tileHeight in tileHeights)
//            {
//                RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
//            }
//        }
//
//        public void BuildHollow(int x, int y, int bottomDepth, int ringWidth)
//        {
//            if (bottomDepth >= 0)
//            {
//                Debug.LogWarning("Building hollow with 0 or higher bottom depth.");
//                return;
//            }
//
//            var tileHeights = MountainBuilder.BuildHollow(x, y, bottomDepth, ringWidth, _xResolution, _zResolution);
//            foreach (var tileHeight in tileHeights)
//            {
//                LowerTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
//            }
//        }
//
//        public void SetYPositionOfMiddleVertices()
//        {
//            for (var i = 0; i < _numberOfVertices; i += 5)
//            {
//                var middleVertice = i + 4;
//
//                var totalHeight = 0f;
//                for (var j = i; j < i + 4; j++)
//                {
//                    totalHeight += _vertices[j].y;
//                }
//
//                _vertices[middleVertice] = new Vector3(_vertices[middleVertice].x, totalHeight / 4f,
//                    _vertices[middleVertice].z);
//            }
//        }
//
//        public void CommitChanges()
//        {
//            SetYPositionOfMiddleVertices();
//            _commitChangesCallback();
//        }
//
//        private bool IsValidPointOnMap(int x, int y)
//        {
//            return MapOperationValidator.IsValidPointOnMap(x, y, _xResolution, _zResolution);
//        }
//    }
}