using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class NoiseSettings : UpdatableData
{
	public Noise.NormalizeMode normalizeMode;

	public float scale = 50;

	[Range(1, 20)]
	public int octaves = 6;
	[Range(0, 1)]
	public float persistance = .6f;
	[Range(1, 20)]
	public float lacunarity = 2;

	public int seed;
	public Vector2 offset;

	public AnimationCurve noiseCurve;

#if UNITY_EDITOR

	protected override void OnValidate()
	{
		ValidateValues();
		base.OnValidate();
	}

	public void ValidateValues()
	{
		scale = Mathf.Max(scale, 0.01f);
		octaves = Mathf.Max(octaves, 1);
		lacunarity = Mathf.Max(lacunarity, 1);
		persistance = Mathf.Clamp01(persistance);
	}
#endif


}