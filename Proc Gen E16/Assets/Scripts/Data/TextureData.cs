using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class TextureData : UpdatableData {

	public Color[] baseColours;
	[Range(0,1)]
	public float[] baseStartHeights;

	float savedMinHeight;
	float savedMaxHeight;

	public void ApplyToMaterial(Material material) {

		material.SetInt ("baseColourCount", baseColours.Length);
		material.SetColorArray ("baseColours", baseColours);
		material.SetFloatArray ("baseStartHeights", baseStartHeights);

		UpdateMeshHeights (material, savedMinHeight, savedMaxHeight);
	}

	public void UpdateMeshHeights(Material material, float minHeight, float maxHeight) {
		savedMinHeight = minHeight;
		savedMaxHeight = maxHeight;

		material.SetFloat ("minHeight", minHeight);
		material.SetFloat ("maxHeight", maxHeight);
	}

}
