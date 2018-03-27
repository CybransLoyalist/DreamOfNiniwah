using UnityEngine;

namespace Assets.Scripts.MapMeshGenerating
{
    public class Vertex
    {
        public Vector3 Location
        {
            get { return Chunk.Vertices[Index]; }
        }

        public IMapChunk Chunk { get; set; }
        public int Index { get; set; }
        public bool IsMiddle { get; set; }

        public void Raise(float height)
        {
            if (height > 0)
            {
                AssingHeight(height);
            }
        }

        public void Lower(float height)
        {
            if (height < 0)
            {
                AssingHeight(height);
            }
        }

        private void AssingHeight(float height)
        {
           // if (height > Chunk.Vertices[Index].y)
            //{
                Chunk.Vertices[Index] = new Vector3(Chunk.Vertices[Index].x, height, Chunk.Vertices[Index].z);

//            }
//            else
//            {
//                Chunk.Vertices[Index] = new Vector3(Chunk.Vertices[Index].x, Chunk.Vertices[Index].y + height, Chunk.Vertices[Index].z);
//
//            }
        }

        public void SetColor(Color color)
        {
            Chunk.VerticesColors[Index] = color;
        }

        public void ChangeTileHeight(float height)
        {
            AssingHeight(height);
        }
    }
}