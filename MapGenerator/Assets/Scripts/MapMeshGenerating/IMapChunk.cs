using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapMeshGenerating
{
    public interface IMapChunk
    {
        List<Color32> VerticesColors { get; }
        List<Vector3> Vertices { get;  }
        void CommitChanges();
//        int XLocationAmongstChunks { get; set; }
//        int ZLocationAmongstChunks { get; set; }
    }
}