using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Data", fileName = "LevelData_")]
public class LevelDataAsset : ScriptableObject
{
    [Header("Thông tin Level")]
    public string levelName = "Level 1";

    [Tooltip("Danh sách các ô đất trong level này")]
    public List<TerrainTileData> tiles = new List<TerrainTileData>();
}
