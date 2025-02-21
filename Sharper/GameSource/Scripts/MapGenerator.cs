using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Systems.Backend;
namespace Gamespace
{
    public static class MapGenerator
    {
        public static Texture2D PerlinNoiseTexture(int size, float scale, int octaves = 4)
        {
            Random rand = new Random();
            PerlinNoise noise = new PerlinNoise(rand.Next());
            Texture2D noiseTexture = new Texture2D(RenderingSystem.Instance._graphics.GraphicsDevice,size,size);
            Color[] pixelColors = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float noiseValue = GetPerlin(x * scale, y * scale,noise, octaves);
                    noiseValue = (noiseValue + 1) / 2; // Normalize from [-1,1] to [0,1]
                    byte colorValue = (byte)(noiseValue * 255);
                    pixelColors[y * size + x] = new Color(colorValue, colorValue, colorValue);
                }
            }
            noiseTexture.SetData(pixelColors);
            return noiseTexture;
        }

        static float GetPerlin(float x, float y, PerlinNoise perlin, int octaves = 4)
        {
            float total = 0;
            float frequency = 0.005f;  // Controls feature size (adjust to avoid grid artifacts)
            float amplitude = 1;
            float persistence = 0.5f;  // Controls how much each octave contributes
            float maxValue = 0;  // Used for normalization

            for (int i = 0; i < octaves; i++)
            {
                total += perlin.Noise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                frequency *= 2.1f;  // Increase frequency (smaller details)
                amplitude *= persistence;  // Reduce amplitude for higher octaves
            }

            return (total / maxValue + 1) / 2;  // Normalize to [0,1]
        }
    }


    public class PerlinNoise
    {
        private int[] _permutation;

        public PerlinNoise(int seed)
        {
            Random rand = new Random(seed);
            _permutation = Enumerable.Range(0, 256).OrderBy(x => rand.Next()).ToArray();
            _permutation = _permutation.Concat(_permutation).ToArray();
        }

        private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
        private static float Lerp(float a, float b, float t) => a + t * (b - a);
        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 7;
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? 2f * v : -2f * v);
        }

        public float Noise(float x, float y)
        {
            int X = (int)MathF.Floor(x) & 255;
            int Y = (int)MathF.Floor(y) & 255;

            x -= MathF.Floor(x);
            y -= MathF.Floor(y);

            float u = Fade(x);
            float v = Fade(y);

            int aa = _permutation[X] + Y;
            int ab = _permutation[X + 1] + Y;
            int ba = _permutation[X] + Y + 1;
            int bb = _permutation[X + 1] + Y + 1;

            return Lerp(Lerp(Grad(_permutation[aa], x, y), Grad(_permutation[ba], x - 1, y), u),
                        Lerp(Grad(_permutation[ab], x, y - 1), Grad(_permutation[bb], x - 1, y - 1), u),
                        v);
        }
    }
}
