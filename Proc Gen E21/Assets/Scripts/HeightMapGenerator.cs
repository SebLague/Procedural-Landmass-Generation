using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HeightMapGenerator {

	public static HeightMap GenerateHeightMap(int width, int height, FalloffGenerator.FallOffType fallOffType, HeightMapSettings heightMapSettings, Vector2 sampleCentre) {
		float[,,] noiseLayers = new float[heightMapSettings.noiseLayers.Length, width, height];
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(width, fallOffType, heightMapSettings.fallOffIntensity);
        for (int k = 0; k < heightMapSettings.noiseLayers.Length; k++)
		{
            AnimationCurve noiseCurve_threadsafe = new AnimationCurve(heightMapSettings.noiseLayers[k].noiseSettings.noiseCurve.keys);
			float[,] values = Noise.GenerateNoiseMap(width, height, heightMapSettings.noiseLayers[k].noiseSettings, sampleCentre);

            for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					noiseLayers[k, i, j] = noiseCurve_threadsafe.Evaluate(values[i, j]);
				}
			}
		}

        float minValue = float.MaxValue;
		float maxValue = float.MinValue;
        AnimationCurve heightCurve_threadsafe = new AnimationCurve(heightMapSettings.heightCurve.keys);

        float[,] heightFinalValues = new float[width, height];
        for (int i = 0; i < width; i++)
        {
			for (int j = 0; j < height; j++)
			{
				float noiseSum = 0;
				float maxPossibleVal = 0;
				for (int k = 0; k < heightMapSettings.noiseLayers.Length; k++)
				{
					noiseSum += noiseLayers[k, i, j] * heightMapSettings.noiseLayers[k].strength;
					maxPossibleVal += heightMapSettings.noiseLayers[k].strength;
                }
				
                heightFinalValues[i,j] = heightCurve_threadsafe.Evaluate((noiseSum / maxPossibleVal) * falloffMap[i, j]) * heightMapSettings.heightMultiplier;

                if (heightFinalValues[i, j] > maxValue) {
					maxValue = heightFinalValues[i, j];
				}
				if (heightFinalValues[i, j] < minValue) {
					minValue = heightFinalValues[i, j];
				}
			}
        }

        return new HeightMap (heightFinalValues, minValue, maxValue);
	}

}

public struct HeightMap {
	public readonly float[,] values;
	public readonly float minValue;
	public readonly float maxValue;

	public HeightMap (float[,] values, float minValue, float maxValue)
	{
		this.values = values;
		this.minValue = minValue;
		this.maxValue = maxValue;
	}
}

