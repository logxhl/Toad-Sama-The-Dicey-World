using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager_HoldMove : MonoBehaviour
{
    public static UIManager_HoldMove Instance;
    public GridPlayerMovement player;

    [Header("Tốc độ di chuyển khi giữ nút")]
    public float holdInterval = 0.25f; // thời gian giữa mỗi bước di chuyển

    private Vector2Int holdDir = Vector2Int.zero;
    private float holdTimer = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (player == null || holdDir == Vector2Int.zero) return;

        // Nếu đang giữ nút
        holdTimer -= Time.deltaTime;

        // Khi đủ thời gian thì gửi lệnh di chuyển
        if (holdTimer <= 0f && !player.IsMoving)
        {
            player.TryMoveSmooth(holdDir); // gọi hàm di chuyển mượt trong Player
            holdTimer = holdInterval;
        }
    }

    // Gọi khi bắt đầu giữ nút
    public void HoldUp() => StartHold(Vector2Int.up);
    public void HoldDown() => StartHold(Vector2Int.down);
    public void HoldLeft() => StartHold(Vector2Int.left);
    public void HoldRight() => StartHold(Vector2Int.right);

    public void ReleaseButton() => StopHold();

    private void StartHold(Vector2Int dir)
    {
        holdDir = dir;
        holdTimer = 0f;
    }

    private void StopHold()
    {
        holdDir = Vector2Int.zero;
    }
}

