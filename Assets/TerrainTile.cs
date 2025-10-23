using UnityEngine;

public class TerrainTile : MonoBehaviour
{
    [Header("Dot / Obstacle Settings")]
    public int dotCount = 0;             // số chấm (0–6)
    public GameObject dotPrefab;         // prefab chấm nhỏ
    public float dotOffset = 0.25f;      // khoảng cách chấm

    private Transform dotsParent;

    void Start()
    {
        GenerateDots();
    }

    private void GenerateDots()
    {
        if (dotPrefab == null || dotCount <= 0) return;

        // Nếu có sẵn object chứa chấm thì xóa cũ
        if (dotsParent != null)
            Destroy(dotsParent.gameObject);

        dotsParent = new GameObject("Dots").transform;
        dotsParent.SetParent(transform);
        dotsParent.localPosition = Vector3.zero;

        Vector3[] positions = GetDotPositions(dotCount);
        foreach (var pos in positions)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            dot.transform.localPosition = pos + Vector3.up * 0.1f;
            dot.transform.SetParent(dotsParent);
        }
    }

    /// <summary>
    /// Trả về các vị trí sắp xếp chấm như mặt xúc xắc (1–6)
    /// </summary>
    private Vector3[] GetDotPositions(int count)
    {
        float o = dotOffset;
        switch (count)
        {
            case 1: return new[] { Vector3.zero };
            case 2: return new[] { new Vector3(-o, 0, -o), new Vector3(o, 0, o) };
            case 3: return new[] { new Vector3(-o, 0, -o), Vector3.zero, new Vector3(o, 0, o) };
            case 4:
                return new[]
            {
                new Vector3(-o, 0, -o), new Vector3(o, 0, -o),
                new Vector3(-o, 0, o),  new Vector3(o, 0, o)
            };
            case 5:
                return new[]
            {
                new Vector3(-o, 0, -o), new Vector3(o, 0, -o),
                Vector3.zero,
                new Vector3(-o, 0, o),  new Vector3(o, 0, o)
            };
            case 6:
                return new[]
            {
                new Vector3(-o, 0, -o), Vector3.zero, new Vector3(o, 0, -o),
                new Vector3(-o, 0, o),  Vector3.zero + new Vector3(0,0,o), new Vector3(o, 0, o)
            };
            default: return new Vector3[0];
        }
    }

    // Hàm gọi khi player bước vào ô
    public void OnEnterTile()
    {
        // Bạn có thể thêm hiệu ứng ở đây, ví dụ đổi màu ô:
        // GetComponent<Renderer>().material.color = Color.white;
    }

    // Hàm gọi khi player rời ô
    public void OnLeaveTile()
    {
        // Ví dụ khôi phục màu cũ:
        // GetComponent<Renderer>().material.color = originalColor;
    }

    // Kiểm tra xem ô này có thể đi vào tại vị trí cụ thể (nếu có dotCount 1x1)
    //public bool IsBlocked(Vector2Int localPosition)
    //{
    //    // Ví dụ kiểm tra ô bị chặn — tạm thời cứ dotCount > 0 là chặn
    //    return dotCount > 0;
    //}
    public bool IsBlocked(Vector2Int localPos)
    {
        // Nếu không có dot nào => không chặn
        if (dotCount <= 0) return false;

        // Chuyển dotCount thành pattern dựa theo mặt xúc xắc
        // Mỗi dot tương ứng với 1 vị trí bị chặn trong lưới 3x3
        bool[,] blockedPattern = GetDicePattern(dotCount);

        // Nếu vị trí vượt ngoài 3x3, cho phép đi
        if (localPos.x < 0 || localPos.x > 2 || localPos.y < 0 || localPos.y > 2)
            return false;

        // Trả về true nếu ô này có chấm
        return blockedPattern[localPos.x, localPos.y];
    }
    private bool[,] GetDicePattern(int dots)
    {
        bool[,] grid = new bool[3, 3];

        switch (dots)
        {
            case 1: grid[1, 1] = true; break;
            case 2: grid[0, 0] = true; grid[2, 2] = true; break;
            case 3: grid[0, 0] = true; grid[1, 1] = true; grid[2, 2] = true; break;
            case 4: grid[0, 0] = true; grid[0, 2] = true; grid[2, 0] = true; grid[2, 2] = true; break;
            case 5:
                grid[0, 0] = true; grid[0, 2] = true;
                grid[1, 1] = true;
                grid[2, 0] = true; grid[2, 2] = true;
                break;
            case 6:
                grid[0, 0] = true; grid[0, 1] = true; grid[0, 2] = true;
                grid[2, 0] = true; grid[2, 1] = true; grid[2, 2] = true;
                break;
        }

        return grid;
    }



}
