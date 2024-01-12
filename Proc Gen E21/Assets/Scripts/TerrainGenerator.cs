﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {

	const float viewerMoveThresholdForChunkUpdate = 10f;
	const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;


	public int colliderLODIndex;
	public LODInfo[] detailLevels;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

	public Transform viewer;
	public Material mapMaterial;

	public Vector2 mapSize;
	Vector2 mapSizeInChunks;

	Vector2 viewerPosition;
	Vector2 viewerPositionOld;

	float meshWorldSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

	void Start() {

		textureSettings.ApplyToMaterial (mapMaterial);
		textureSettings.UpdateMeshHeights (mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

		float maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;
		meshWorldSize = meshSettings.meshWorldSize;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

		mapSizeInChunks = new Vector2(Mathf.RoundToInt(mapSize.x / meshWorldSize), Mathf.RoundToInt(mapSize.y / meshWorldSize));

		UpdateVisibleChunks ();
	}

	void Update() {
		viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);

		if (viewerPosition != viewerPositionOld) {
			foreach (TerrainChunk chunk in visibleTerrainChunks) {
				chunk.UpdateCollisionMesh ();
			}
		}

		if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks ();
		}
	}
		
	void UpdateVisibleChunks() {
		HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2> ();
		for (int i = visibleTerrainChunks.Count-1; i >= 0; i--) {
			alreadyUpdatedChunkCoords.Add (visibleTerrainChunks [i].coord);
			visibleTerrainChunks [i].UpdateTerrainChunk ();
		}
			
		int currentChunkCoordX = Mathf.RoundToInt (viewerPosition.x / meshWorldSize);
		int currentChunkCoordY = Mathf.RoundToInt (viewerPosition.y / meshWorldSize);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				if (!alreadyUpdatedChunkCoords.Contains (viewedChunkCoord)) {
					if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
						terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					} else {
						AddNewChunk (viewedChunkCoord);
					}
				}

			}
		}
	}

	void AddNewChunk(Vector2 viewedChunkCoord)
	{
		FalloffGenerator.FallOffType fallOffType = FalloffGenerator.FallOffType.None;
		if (Mathf.Abs(viewedChunkCoord.x) > mapSizeInChunks.x + 1 || Mathf.Abs(viewedChunkCoord.y) > mapSizeInChunks.y + 1) {
            fallOffType = FalloffGenerator.FallOffType.Ocean;
        }
        else if (viewedChunkCoord.x == mapSizeInChunks.x + 1)
		{
			if (viewedChunkCoord.y == mapSizeInChunks.y + 1)
			{
                fallOffType = FalloffGenerator.FallOffType.NECoast;
            }
            else if (viewedChunkCoord.y == -mapSizeInChunks.y - 1)
            {
                fallOffType = FalloffGenerator.FallOffType.SECoast;
            }
            else fallOffType = FalloffGenerator.FallOffType.EastCoast;
		}
        else if (viewedChunkCoord.x == -mapSizeInChunks.x - 1)
        {
            if (viewedChunkCoord.y == -mapSizeInChunks.y - 1)
            {
                fallOffType = FalloffGenerator.FallOffType.SWCoast;
            }
            else if (viewedChunkCoord.y == mapSizeInChunks.y + 1)
            {
                fallOffType = FalloffGenerator.FallOffType.NWCoast;
            }
            else fallOffType = FalloffGenerator.FallOffType.WestCoast;
        }
        else if (viewedChunkCoord.y == mapSizeInChunks.y + 1)
        {
            fallOffType = FalloffGenerator.FallOffType.NorthCoast;
        }
        else if (viewedChunkCoord.y == -mapSizeInChunks.y - 1)
        {
            fallOffType = FalloffGenerator.FallOffType.SouthCoast;
        }

        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, fallOffType, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
        newChunk.Load();
    }

	void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
		if (isVisible) {
			visibleTerrainChunks.Add (chunk);
		} else {
			visibleTerrainChunks.Remove (chunk);
		}
	}

}

[System.Serializable]
public struct LODInfo {
	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int lod;
	public float visibleDstThreshold;


	public float sqrVisibleDstThreshold {
		get {
			return visibleDstThreshold * visibleDstThreshold;
		}
	}
}
