using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapMeshGenerating;
using Assets.Scripts.MapUtilities;
using UnityEngine;

namespace Assets.Scripts.MapChanging
{
    public class MapChanger : IMapChanger
    {
        public int ZResolution { get; private set; }
        public int XResolution {get; private set; }
        public Dictionary<Vector2Int, Tile> Tiles { get; set; }
        private readonly MapChunksAccessor _mapChunksAccessor;

        public MapChanger(
            int xResolution,
            int zResolution,
            MapChunksAccessor mapChunksAccessor)
        {
            XResolution = xResolution;
            ZResolution = zResolution;
            _mapChunksAccessor = mapChunksAccessor;

            Tiles = new Dictionary<Vector2Int, Tile>();
            for (int x = 0; x < xResolution; x++)
            {
                for (int z = 0; z < ZResolution; z++)
                {
                    var tile = new Tile() {X = x, Z = z};
                    Tiles[new Vector2Int(x, z)] = tile;
                    mapChunksAccessor.OwnTileVertices[tile] = new List<Vertex>();
                }
            }
        }

        public void ColorTileExact(int x, int y, Color color)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                ColorMiddleVertexOfTile(tile, color);
            }
        }

        public void ChangeTileHeight(int x, int y, float height)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in _mapChunksAccessor.AllTileVertices[tile])
                {
                    tileVertex.ChangeTileHeight(height);
                }
            }
        }

        public void RaiseTile(int x, int y, float height)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in _mapChunksAccessor.AllTileVertices[tile])
                {
                    tileVertex.Raise(height);
                }
            }
        }

        public void LowerTile(int x, int y, float height)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in _mapChunksAccessor.AllTileVertices[tile])
                {
                    tileVertex.Lower(height);
                }
            }
        }

        public void ColorTile(int x, int y, Color color)
        {
            if (IsValidPointOnMap(x, y))
            {
                var tile = Tiles[new Vector2Int(x, y)];
                foreach (var tileVertex in _mapChunksAccessor.OwnTileVertices[tile])
                {
                    tileVertex.SetColor(color);
                }
                foreach (var neighbour in GetNeighbours(tile))
                {
                    ColorMiddleVertexOfTile(neighbour, color);
                }
            }
        }


        public void BuildMountain(int x, int y, int levelsCount, int ringWidth, float ringHeight)
        {
            var baseHeight = _mapChunksAccessor.GetHeightOfTileAt(Tiles[new Vector2Int(x,y)]);
            var tileHeights = MountainBuilder.BuildMountain(x, y, levelsCount, ringWidth, ringHeight, XResolution, ZResolution);
            foreach (var tileHeight in tileHeights)
            {
                RaiseTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value + baseHeight);
            }
        }

        public void BuildHollow(int x, int y, int levelsCount, int ringWidth, float ringHeight)
        {
            var tileHeights = MountainBuilder.BuildHollow(x, y, levelsCount, ringWidth, ringHeight, XResolution, ZResolution);
            foreach (var tileHeight in tileHeights)
            {
                LowerTile(tileHeight.Key.x, tileHeight.Key.y, tileHeight.Value);
            }
        }

        private void ColorMiddleVertexOfTile(Tile tile, Color color)
        {
            var middleVertex = _mapChunksAccessor.AllTileVertices[tile].First(a => a.IsMiddle);
            middleVertex.SetColor(color);
        }

        private readonly Dictionary<Tile, List<Tile>> _neighboursRelations = new Dictionary<Tile, List<Tile>>();
        public List<Tile> GetNeighbours(Tile tile, NeighbourMode neighbourMode = NeighbourMode.Orthogonal)
        {
            if (!_neighboursRelations.ContainsKey(tile))
            {
                var x = tile.X;
                var y = tile.Z;
                var result = new List<Tile>();
                if (x - 1 >= 0)
                {
                    result.Add(Tiles[new Vector2Int(x - 1, y)]);
                }
                if (x + 1 < XResolution)
                {
                    result.Add(Tiles[new Vector2Int(x + 1, y)]);
                }
                if (y - 1 >= 0)
                {
                    result.Add(Tiles[new Vector2Int(x, y - 1)]);
                }
                if (y + 1 < ZResolution)
                {
                    result.Add(Tiles[new Vector2Int(x, y + 1)]);
                }
                if (neighbourMode == NeighbourMode.All)
                {
                    if (x - 1 >= 0 && y - 1 >= 0)
                    {
                        result.Add(Tiles[new Vector2Int(x - 1, y - 1)]);
                    }
                    if (x + 1 < XResolution && y + 1 < ZResolution)
                    {
                        result.Add(Tiles[new Vector2Int(x + 1, y + 1)]);
                    }
                    if (x + 1 < XResolution && y - 1 >= 0)
                    {
                        result.Add(Tiles[new Vector2Int(x + 1, y - 1)]);
                    }
                    if (x - 1 >= 0 && y + 1 < ZResolution)
                    {
                        result.Add(Tiles[new Vector2Int(x - 1, y + 1)]);
                    }
                }

                _neighboursRelations[tile] = result;
            }
            return _neighboursRelations[tile]; ;
        }

        private static List<Color> _colors = new List<Color>{Color.blue, Color.black, Color.cyan, Color.gray, Color.magenta, Color.green, Color.red};
        public void ColorRingsAround(int x, int y, int ringsCount, int ringWidth)
        {
            var rings = MapPiecesSelector.GetRingsAround(x, y, ringsCount, ringWidth, XResolution, ZResolution);

            foreach (var ring in rings)
            {
                foreach (var tile in ring.Value)
                {

                    ColorTileExact(tile.x, tile.y, _colors[ring.Key]);
                }
            }
        }

        public HashSet<Vector2Int> GetCircle(int x, int y, int radius)
        {
            var circle = MapPiecesSelector.GetRingsAround(x, y, 0, radius, XResolution, ZResolution);

            return new HashSet<Vector2Int>(circle[0]);
        }

        private bool IsValidPointOnMap(int x, int y)
        {
            return MapOperationValidator.IsValidPointOnMap(x, y, XResolution, ZResolution);
        }
    }
}