using UnityEngine;
using System.Collections;

public static class Noise {

	public enum NormalizeMode {Global};

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre) {
		float[,] noiseMap = new float[mapWidth,mapHeight];

		System.Random prng = new System.Random (settings.seed);
		Vector2[] octaveOffsets = new Vector2[settings.octaves];

		float maxPossibleHeight = 0;
		float amplitude = 1;
		float frequency = 1;

		for (int i = 0; i < settings.octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + settings.offset.x + sampleCentre.x;
			float offsetY = prng.Next (-100000, 100000) - settings.offset.y - sampleCentre.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);

			maxPossibleHeight += amplitude;
			amplitude *= settings.persistance;
		}

		float halfWidth = mapWidth / 2f;
		float halfHeight = mapHeight / 2f;

		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				amplitude = 1;
				frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < settings.octaves; i++) {
					float sampleX = (x-halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
					float sampleY = (y-halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) - 0.5f;
					noiseHeight += perlinValue * amplitude;

					amplitude *= settings.persistance;
					frequency *= settings.lacunarity;
				}

				noiseMap [x, y] = noiseHeight;

				if (settings.normalizeMode == NormalizeMode.Global) {
					float normalizedHeight = (noiseMap [x, y] + (maxPossibleHeight/2)) / maxPossibleHeight;
					noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, 1f);
				}
			}
		}

        return noiseMap;
	}

}

