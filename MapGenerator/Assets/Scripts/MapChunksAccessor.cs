using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public class MapChunksAccessor
    {
        public Dictionary<Tile, List<Vertex>> OwnTileVertices;
        public Dictionary<Tile, List<Vertex>> AllTileVertices;

        public MapChunksAccessor()
        {
            OwnTileVertices = new Dictionary<Tile, List<Vertex>>();
            AllTileVertices = new Dictionary<Tile, List<Vertex>>();
        }

        public void Recalculate()
        {
            foreach (var tileVertex in OwnTileVertices)
            {
                AllTileVertices[tileVertex.Key] = tileVertex.Value.ToList();
            }

            foreach (var tileVertex1 in AllTileVertices)
            {
                var tile = tileVertex1.Key;
                var neighbours = OwnTileVertices.Where(a =>
                    a.Key.X == tile.X - 1 && a.Key.Z == tile.Z ||
                    a.Key.X == tile.X + 1 && a.Key.Z == tile.Z ||
                    a.Key.X == tile.X  && a.Key.Z == tile.Z + 1 ||
                    a.Key.X == tile.X && a.Key.Z == tile.Z - 1 ||

                    a.Key.X == tile.X - 1 && a.Key.Z == tile.Z - 1 ||
                    a.Key.X == tile.X - 1 && a.Key.Z == tile.Z + 1 ||
                    a.Key.X == tile.X + 1 && a.Key.Z == tile.Z - 1 ||
                    a.Key.X == tile.X + 1 && a.Key.Z == tile.Z + 1 );

                foreach (var keyValuePair in neighbours)
                {
                    foreach (var vertex in keyValuePair.Value)
                    {
                        if (tileVertex1.Value.Any(a => a.Vector3 == vertex.Vector3))
                        {
                            if (!tileVertex1.Value.Contains(vertex))
                            {
                                tileVertex1.Value.Add(vertex);
                            }
                        }
                    }
                }

            }

        }
    }
}