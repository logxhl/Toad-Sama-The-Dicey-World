using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Data", fileName = "LevelData_")]
public class LevelDataAsset : ScriptableObject
{
    [Header("Thông tin Level")]
    public string levelName = "Level 1";

    [Tooltip("Danh sách các ô đất trong level này")]
    public List<TerrainTileData> tiles = new List<TerrainTileData>();
    // Thêm danh sách vị trí coin (tọa độ x,z)
    public List<Vector2Int> coinPositions = new List<Vector2Int>();
    // Vị trí player khi bắt đầu level
    public Vector2Int playerStartPos = Vector2Int.zero;
}
