using UnityEngine;

public class GridPlayerMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1f;
    public float checkHeight = 2f;
    public LayerMask groundLayer;
    public string[] walkableTags = { "Green", "Red", "Blue", "Yellow", "Start" };

    private Vector2Int gridPos;          // vị trí tuyệt đối của player trong lưới
    private GameObject lastGround;       // ô đất hiện tại
    private TerrainTile currentTile;     // tile mà player đang đứng trên
    private Vector2Int localPos;         // vị trí trong 3×3 của tile hiện tại (0..2,0..2)

    void Start()
    {
        gridPos.x = Mathf.RoundToInt(transform.position.x / cellSize);
        gridPos.y = Mathf.RoundToInt(transform.position.z / cellSize);
        UpdatePlayerPosition();

        lastGround = GetCurrentGround();
        if (lastGround != null)
            currentTile = lastGround.GetComponent<TerrainTile>();
    }

    public void MoveUp() => TryMove(Vector2Int.up);
    public void MoveDown() => TryMove(Vector2Int.down);
    public void MoveLeft() => TryMove(Vector2Int.left);
    public void MoveRight() => TryMove(Vector2Int.right);

    private void TryMove(Vector2Int dir)
    {
        Vector2Int target = gridPos + dir;
        Vector3 targetPos = new Vector3(target.x * cellSize, transform.position.y + checkHeight, target.y * cellSize);

        // Raycast kiểm tra có ô đất không
        if (!Physics.Raycast(targetPos, Vector3.down, out RaycastHit hit, checkHeight * 2, groundLayer))
            return;

        GameObject ground = hit.collider.gameObject;

        // Kiểm tra tag hợp lệ
        bool canWalk = false;
        foreach (string tag in walkableTags)
            if (ground.CompareTag(tag)) { canWalk = true; break; }
        if (!canWalk) return;

        // Lấy tile của ô này
        TerrainTile tile = ground.GetComponent<TerrainTile>();
        if (tile == null) return;

        // Tính vị trí con (local 3x3)
        Vector3 localOffset = targetPos - ground.transform.position;
        int lx = Mathf.Clamp(Mathf.RoundToInt(localOffset.x / (cellSize / 1f)) + 1, 0, 2);
        int ly = Mathf.Clamp(Mathf.RoundToInt(localOffset.z / (cellSize / 1f)) + 1, 0, 2);
        Vector2Int nextLocal = new(lx, ly);

        // Kiểm tra chướng ngại vật cụ thể (dot)
        if (tile.IsBlocked(nextLocal))
        {
            Debug.Log($"Ô {ground.name} bị chặn tại vị trí {nextLocal}");
            return;
        }

        // Nếu player rời tile cũ
        if (currentTile != tile && currentTile != null)
            currentTile.OnLeaveTile();

        // Di chuyển player
        gridPos = target;
        UpdatePlayerPosition();

        // Cập nhật tile mới
        currentTile = tile;
        tile.OnEnterTile();
        lastGround = ground;
    }

    private void UpdatePlayerPosition()
    {
        transform.position = new Vector3(gridPos.x * cellSize, transform.position.y, gridPos.y * cellSize);
    }

    private GameObject GetCurrentGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up * checkHeight, Vector3.down, out RaycastHit hit, checkHeight * 2, groundLayer))
            return hit.collider.gameObject;
        return null;
    }
    public void ForceSyncPosition()
    {
        gridPos.x = Mathf.RoundToInt(transform.position.x / cellSize);
        gridPos.y = Mathf.RoundToInt(transform.position.z / cellSize);
    }

}
