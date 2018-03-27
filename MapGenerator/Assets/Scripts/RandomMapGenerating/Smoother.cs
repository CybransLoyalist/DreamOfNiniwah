using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.RandomMapGenerating
{
    public static class Smoother2
    {
        public static List<Vector2Int> Smooth(List<Vector2Int> pointList)
        {
            List<Vector2Int> smoothedPoints = new List<Vector2Int>();

            for (int i = 1; i < pointList.Count; i++)
            {
                if (Vector2Int.Distance(pointList[i - 1], pointList[i]) < 15f)
                {
                    pointList.RemoveAt(i);
                    i--;
                }
            }

            if (pointList.Count < 4) return null;

            smoothedPoints.Add(pointList[0]);

            for (int i = 1; i < pointList.Count - 2; i++)
            {
                smoothedPoints.Add(pointList[i]);

                smoothedPoints.Add(CatmullRom(pointList[i - 1], pointList[i], pointList[i + 1], pointList[i + 2], .5f));
                //smoothedPoints.Add(Vector2.CatmullRom(pointList[i - 1], pointList[i], pointList[i + 1], pointList[i + 2], .2f));
                //smoothedPoints.Add(Vector2.CatmullRom(pointList[i - 1], pointList[i], pointList[i + 1], pointList[i + 2], .3f));
                //smoothedPoints.Add(Vector2.CatmullRom(pointList[i - 1], pointList[i], pointList[i + 1], pointList[i + 2], .7f));
                //smoothedPoints.Add(Vector2.CatmullRom(pointList[i - 1], pointList[i], pointList[i + 1], pointList[i + 2], .8f));
                //smoothedPoints.Add(Vector2.CatmullRom(pointList[i - 1], pointList[i], pointList[i + 1], pointList[i + 2], .9f));
            }

            smoothedPoints.Add(pointList[pointList.Count - 2]);
            smoothedPoints.Add(pointList[pointList.Count - 1]);

            pointList.Clear();
            pointList.AddRange(smoothedPoints);

            return smoothedPoints;
        }

        private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return pos;
        }

        private static Vector2Int CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            Vector2 a = 2 * p1;
            Vector2 b = p2 - p0;
            Vector2 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector2 d = -p0 + 3f * p1 - 3f * p2 + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector2 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            return new Vector2Int((int)pos.x, (int)pos.y);
        }

    }
}