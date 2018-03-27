using System;
using System.Diagnostics;
using Assets.Scripts.MapChanging;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.RandomMapGenerating
{
    public class RandomMapGenerator
    {
        private readonly IMapChanger _mapChanger;
        private Random _random;
        private RiverGenerator _riverGenerator;

        public RandomMapGenerator(IMapChanger mapChanger, int? seed = null)
        {
            _mapChanger = mapChanger;
            if (seed == null)
            {
                seed = new Random().Next(Int32.MaxValue);
            }
            _random = new Random(seed.Value);
            _riverGenerator = new RiverGenerator(_random, _mapChanger);
        }

        public void GenerateFor()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            //  BuildInitialSlope(peak);
            //            BuildHills();
            _riverGenerator.BuildRiver();

            stopwatch.Stop();
            var time = stopwatch.ElapsedMilliseconds;
        }

        private void BuildInitialSlope(Vector2Int peak)
        {
            int levelsCount = _random.Next(5, 9);
            int ringWidth = _random.Next(15, 25);
            float ringHeight = 0.1f + (float) (_random.NextDouble());
            _mapChanger.BuildMountain(peak.x, peak.y, levelsCount, ringWidth, ringHeight);
        }


        private void BuildHills()
        {
            var seedX = _random.Next(0, _mapChanger.XResolution);
            var seedZ = _random.Next(0, _mapChanger.ZResolution);

//            var ring = _mapChanger.GetCircle(50, 50, 50);
//            var ring2 = _mapChanger.GetCircle(150, 150, 50);

            for (int i = 0; i < _mapChanger.XResolution; ++i)
            {
                for (int j = 0; j < _mapChanger.ZResolution; j++)
                {
                    float xVal = ((float) i / _mapChanger.XResolution);
                    float zVal = ((float) j / _mapChanger.ZResolution);


                    // var multiplier = ring.Contains(new Vector2Int(i, j)) || ring2.Contains(new Vector2Int(i, j)) ? 1.5f : 0.5f;
                    var perlin = GetFractalNoise(xVal, zVal, seedX, seedZ) +
                                 ((i + j) / (float) _mapChanger.XResolution);
                    _mapChanger.ChangeTileHeight(i, j, perlin);
                }
            }
            //_mapChanger.BuildMountain(20, 20, 5, 4, 1f);
//            _mapChanger.BuildMountain(60, 60, 5, 5, 1f);
        }

        public float GetPerlinNoise(float x, float z)
        {
            var perlinNoise = Mathf.PerlinNoise(x, z); //returns value between 0 and 1, but we prefere from -1 to 1
            return perlinNoise * 2 - 1;
        }

        private readonly int _octaves = 2; //wieksze aplitudy i ostrzejsze gorki
        private readonly float _lacunarity = 2; //im wiekszy tym wiecej malych faldek
        private readonly float _gain = 6; //im wiekszy tym kazda gorka wyzsza
        private readonly float _perlinScale = 2f; //im wieszy tym wiecej górek na jednostke powierzchni
   

        public float GetFractalNoise(float x, float z, int seedX, int seedZ)
        {
            float fractalNoise = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < _octaves; i++)
            {
                float xVal = x * frequency * _perlinScale * (_mapChanger.XResolution / 100f);
                float zVal = z * frequency * _perlinScale * (_mapChanger.XResolution / 100f);

                fractalNoise += amplitude * GetPerlinNoise(seedX + xVal, seedZ + zVal);
                frequency *= _lacunarity;
                amplitude *= _gain;
            }

            return fractalNoise;
        }
    }
}