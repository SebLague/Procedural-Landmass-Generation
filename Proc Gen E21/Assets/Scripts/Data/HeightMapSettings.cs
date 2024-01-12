using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData {

	public NoiseLayers[] noiseLayers;
	public float heightMultiplier;
	[Range(0f, 1f)]
	public float fallOffIntensity;
	public AnimationCurve heightCurve;

    public float minHeight {
		get {
			return heightMultiplier * heightCurve.Evaluate (0);
		}
	}

	public float maxHeight {
		get {
			return heightMultiplier * heightCurve.Evaluate (1);
		}
	}
}


[System.Serializable]
public struct NoiseLayers
{
	public NoiseSettings noiseSettings;
	[Range(0f, 1f)]
	public float strength;
}