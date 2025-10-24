using System;
using UnityEngine;

[System.Serializable]
public class TerrainTileData
{
    public Vector2Int position;
    public string tag;
    public int dotCount;

    [Header("Coin data")]
    public bool hasCoin;
    public Vector3 coinLocalPos;
}
