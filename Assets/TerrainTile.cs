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

    // 🧱 Tạo lại chấm theo dotCount hiện tại
    public void GenerateDots()
    {
        if (dotPrefab == null) return;

        // Xoá cũ
        if (dotsParent != null)
            DestroyImmediate(dotsParent.gameObject);

        dotsParent = new GameObject("Dots").transform;
        dotsParent.SetParent(transform);
        dotsParent.localPosition = Vector3.zero;

        if (dotCount <= 0) return;

        Vector3[] positions = GetDotPositions(dotCount);
        foreach (var pos in positions)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            dot.transform.localPosition = pos + Vector3.up * 0.1f;
            dot.transform.SetParent(dotsParent);
        }
    }

    // Giảm dotCount khi rời khỏi ô
    public void DecreaseDot()
    {
        if (dotCount > 0)
        {
            dotCount--;
            if (dotCount == 0)
            {
                Debug.Log($"💥 Ô {gameObject.name} bị sụp đổ!");
                Destroy(gameObject); // xoá hẳn tile
            }
            else
            {
                // Tạo lại chấm mới
                GenerateDots();
                Debug.Log($"❄ Ô {gameObject.name} (Blue) giảm xuống còn {dotCount} chấm");
            }
        }
    }

    // ✅ Khi player rời tile
    public void OnLeaveTile()
    {
        // Nếu là đất Tuyết (Blue), giảm 1 dot
        if (CompareTag("Blue"))
            DecreaseDot();
    }

    public void OnEnterTile() { }

    public bool IsBlocked(Vector2Int localPos)
    {
        if (dotCount <= 0) return false;
        bool[,] pattern = GetDicePattern(dotCount);
        if (localPos.x < 0 || localPos.x > 2 || localPos.y < 0 || localPos.y > 2) return false;
        return pattern[localPos.x, localPos.y];
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

    // private Vector3[] GetDotPositions(int count)
    // {
    //     float o = dotOffset;
    //     switch (count)
    //     {
    //         case 1: return new[] { Vector3.zero };
    //         case 2: return new[] { new Vector3(-o, 0, -o), new Vector3(o, 0, o) };
    //         case 3: return new[] { new Vector3(-o, 0, -o), Vector3.zero, new Vector3(o, 0, o) };
    //         case 4:
    //             return new[]
    //             {
    //                 new Vector3(-o,0,-o), new Vector3(o,0,-o),
    //                 new Vector3(-o,0,o), new Vector3(o,0,o)
    //             };
    //         case 5:
    //             return new[]
    //             {
    //                 new Vector3(-o,0,-o), new Vector3(o,0,-o),
    //                 Vector3.zero,
    //                 new Vector3(-o,0,o), new Vector3(o,0,o)
    //             };
    //         case 6:
    //             return new[]
    //             {
    //                 new Vector3(-o,0,-o), new Vector3(0,0,-o), new Vector3(o,0,-o),
    //                 new Vector3(-o,0,o), new Vector3(0,0,o), new Vector3(o,0,o)
    //             };
    //         default: return new Vector3[0];
    //     }
    // }
    private Vector3[] GetDotPositions(int count)
    {
        float o = dotOffset;

        // Đảo trục Z để hiển thị đúng hướng xúc xắc
        Vector3 P(float x, float z) => new Vector3(x, 0, -z);

        switch (count)
        {
            case 1: return new[] { P(0, 0) };
            case 2: return new[] { P(-o, -o), P(o, o) };
            case 3: return new[] { P(-o, -o), P(0, 0), P(o, o) };
            case 4:
                return new[]
                {
                P(-o,-o), P(o,-o),
                P(-o,o),  P(o,o)
            };
            case 5:
                return new[]
                {
                P(-o,-o), P(o,-o),
                P(0,0),
                P(-o,o),  P(o,o)
            };
            case 6:
                return new[]
                {
                P(-o,-o), P(0,-o), P(o,-o),
                P(-o,o),  P(0,o),  P(o,o)
            };
            default: return new Vector3[0];
        }
    }

}
