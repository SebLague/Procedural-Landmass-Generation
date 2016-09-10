using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public int mapWidth;
	public int mapHeight;
	public float noiseScale;

	public bool autoUpdate;

	public void GenerateMap() {
		float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, noiseScale);


		MapDisplay display = FindObjectOfType<MapDisplay> ();
		display.DrawNoiseMap (noiseMap);
	}
	
}
