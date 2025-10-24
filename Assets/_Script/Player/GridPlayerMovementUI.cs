using System.Collections;
using UnityEngine;

public class GridPlayerMovement : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1f;
    public float moveSpeed = 6f;          // tốc độ (lớn = nhanh)
    public float checkHeight = 2f;
    public LayerMask groundLayer;
    public string[] walkableTags = { "Green", "Red", "Blue", "Yellow", "Start" };

    // internal state
    private Vector2Int gridPos;
    private TerrainTile currentTile;
    private GameObject lastGround;
    private bool isMoving = false;

    // expose để UI kiểm tra (read-only)
    public bool IsMoving => isMoving;

    private Coroutine moveCoroutine = null;

    private void Start()
    {
        // init gridPos từ vị trí hiện tại
        gridPos.x = Mathf.RoundToInt(transform.position.x / cellSize);
        gridPos.y = Mathf.RoundToInt(transform.position.z / cellSize);
        UpdatePlayerPosition();

        var ground = GetCurrentGround();
        if (ground) currentTile = ground.GetComponent<TerrainTile>();
    }

    // public API cho UIManager gọi
    public void TryMoveSmooth(Vector2Int dir)
    {
        if (isMoving) return;

        Vector2Int target = gridPos + dir;
        Vector3 checkPos = new Vector3(target.x * cellSize, transform.position.y + checkHeight, target.y * cellSize);

        if (!Physics.Raycast(checkPos, Vector3.down, out RaycastHit hit, checkHeight * 2, groundLayer))
            return;

        GameObject ground = hit.collider.gameObject;

        // tag check
        bool canWalk = false;
        foreach (string tag in walkableTags)
            if (ground.CompareTag(tag)) { canWalk = true; break; }
        if (!canWalk) return;

        TerrainTile newTile = ground.GetComponent<TerrainTile>();
        if (newTile == null) return;

        // tính vị trí local để check dot pattern
        Vector3 localOffset = checkPos - ground.transform.position;
        int lx = Mathf.Clamp(Mathf.FloorToInt((localOffset.x + (cellSize * 0.5f)) / (cellSize / 3f)), 0, 2);
        int ly = Mathf.Clamp(Mathf.FloorToInt((localOffset.z + (cellSize * 0.5f)) / (cellSize / 3f)), 0, 2);
        Vector2Int nextLocal = new Vector2Int(lx, ly);

        if (newTile.IsBlocked(nextLocal))
        {
            Debug.Log($"Ô {ground.name} bị chặn tại {nextLocal}");
            return;
        }

        // bắt đầu di chuyển
        moveCoroutine = StartCoroutine(MoveSmooth(target, newTile));
    }

    private IEnumerator MoveSmooth(Vector2Int target, TerrainTile newTile)
    {
        isMoving = true;

        // Lưu lại tile cũ để xử lý sau
        TerrainTile oldTile = currentTile;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(target.x * cellSize, transform.position.y, target.y * cellSize);
        float t = 0f;
        float duration = 1f / moveSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.position = end;
        gridPos = target;

        // Sau khi di chuyển hoàn toàn → gọi OnLeaveTile cho tile cũ
        if (oldTile != null && oldTile != newTile)
            oldTile.OnLeaveTile();

        // Cập nhật tile mới và gọi OnEnterTile
        currentTile = newTile;
        currentTile.OnEnterTile();

        lastGround = GetCurrentGround();
        isMoving = false;
        moveCoroutine = null;
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

    // gọi để sync gridPos và cập nhật currentTile từ vị trí world hiện tại
    public void ForceSyncPosition()
    {
        gridPos.x = Mathf.RoundToInt(transform.position.x / cellSize);
        gridPos.y = Mathf.RoundToInt(transform.position.z / cellSize);

        lastGround = GetCurrentGround();
        if (lastGround != null)
            currentTile = lastGround.GetComponent<TerrainTile>();
    }

    // phương thức công khai để LevelManager đặt player vào vị trí start an toàn
    public void PlaceAt(Vector3 worldPos)
    {
        // dừng mọi coroutine di chuyển
        StopMovementCoroutines();

        transform.position = worldPos;
        ForceSyncPosition();
    }

    // dừng coroutine di chuyển (gọi trước khi đặt vị trí)
    public void StopMovementCoroutines()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        moveCoroutine = null;
        isMoving = false;
    }
}
