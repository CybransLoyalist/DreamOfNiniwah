using UnityEngine;

namespace Assets.Scripts
{
    public class Vertex
    {
        public Vector3 Vector3
        {
            get { return Chunk.Vertices[Index]; }
        }

        public IMapChunk Chunk { get; set; }
        public int Index { get; set; }
        public bool IsMiddle { get; set; }
    }
}