using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileSoawner : MonoBehaviour
{
    [Header("Land Settings")]
    public GameObject landPrefab;           // Prefab của đất (ví dụ 1 cube)
    public int gridSize = 30;               // Bản đồ nước 30x30
    public float cellSize = 1f;             // Mỗi ô = 1 đơn vị
    public int landSize = 3;                // Kích thước vùng đất (3x3)
    public Vector2Int startPos = new Vector2Int(14, 14); // Góc trái trên
    public float heightAboveWater = 0.05f;  // Độ cao so với mặt nước

    [ContextMenu("Spawn Single Land Block")]
    public void SpawnSingleLandBlock()
    {
        if (landPrefab == null)
        {
            Debug.LogError("landPrefab chưa được gán!");
            return;
        }

        float halfMap = gridSize * cellSize / 2f;

        // Tính vị trí tâm khối đất
        float worldX = -halfMap + (startPos.x + landSize / 2f) * cellSize;
        float worldZ = -halfMap + (startPos.y + landSize / 2f) * cellSize;
        Vector3 pos = new Vector3(worldX, heightAboveWater, worldZ);

#if UNITY_EDITOR
        GameObject land = (GameObject)PrefabUtility.InstantiatePrefab(landPrefab, transform);
#else
        GameObject land = Instantiate(landPrefab, transform);
#endif

        // Scale để khớp 3x3 ô
        land.transform.position = pos;
        land.transform.localScale = new Vector3(landSize * cellSize, 0.2f, landSize * cellSize);
        land.name = $"LandBlock_{startPos.x}_{startPos.y}_{landSize}x{landSize}";

        Debug.Log($"Spawned single land block {landSize}x{landSize} at ({startPos.x},{startPos.y})");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float halfMap = gridSize * cellSize / 2f;
        float worldX = -halfMap + (startPos.x + landSize / 2f) * cellSize;
        float worldZ = -halfMap + (startPos.y + landSize / 2f) * cellSize;
        Vector3 center = new Vector3(worldX, heightAboveWater, worldZ);
        Vector3 size = new Vector3(landSize * cellSize, 0.01f, landSize * cellSize);
        Gizmos.DrawWireCube(center, size);
    }
}
