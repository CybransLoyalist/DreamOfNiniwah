using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.MapChanging;
using Assets.Scripts.MapUtilities;

namespace Assets.Scripts.MapMeshGenerating
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

        public void Recalculate(IMapChanger mapChanger)
        {
            foreach (var tileVertex in OwnTileVertices)
            {
                AllTileVertices[tileVertex.Key] = tileVertex.Value.ToList();
            }

        }

        public float GetHeightOfTileAt(Tile tile)
        {
            var tileVertices = AllTileVertices[tile];

            return tileVertices.Average(a => a.Location.y);
        }
    }
}