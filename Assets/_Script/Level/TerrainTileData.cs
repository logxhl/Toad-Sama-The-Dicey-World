using System;
using UnityEngine;

[Serializable]
public class TerrainTileData
{
    [Tooltip("Tọa độ ô đất trên lưới (x,z)")]
    public Vector2Int position;

    [Tooltip("Loại đất (Green, Blue, Red, Yellow, Start)")]
    public string tag = "Green";

    [Tooltip("Số chấm ban đầu (0-6)")]
    [Range(0, 6)]
    public int dotCount = 0;
}
