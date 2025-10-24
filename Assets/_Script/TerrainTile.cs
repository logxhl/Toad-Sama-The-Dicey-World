using UnityEngine;
using System.Collections.Generic;

public class TerrainTile : MonoBehaviour
{
    [Header("Dot / Obstacle Settings")]
    public int dotCount = 0;             // số chấm (0–6)
    public GameObject dotPrefab;         // prefab chấm nhỏ
    public float dotOffset = 0.25f;      // khoảng cách chấm

    private Transform dotsParent;
    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public List<Vector3> coinPositions = new List<Vector3>(); // vị trí coin local
    private List<GameObject> spawnedCoins = new List<GameObject>();
    public void GenerateCoins()
    {
        if (coinPrefab == null || coinPositions.Count == 0) return;

        foreach (var pos in coinPositions)
        {
            GameObject coin = Instantiate(coinPrefab, transform);
            coin.transform.localPosition = pos + Vector3.up * 0.5f;
            spawnedCoins.Add(coin);
            LevelManager.Instance.RegisterCoin(coin);
        }
    }
    void Start()
    {
        GenerateDots();
        GenerateCoins();
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

    // ✅ Khi player rời tile
    public void OnLeaveTile()
    {
        // ❄ Nếu là đất Tuyết (Blue): giảm 1 dot, nếu hết -> xóa
        if (CompareTag("Blue"))
        {
            DecreaseDot();
            return;
        }

        // 🌞 Nếu là đất Vàng (Yellow): tăng 1 dot, nếu >6 -> xóa
        if (CompareTag("Yellow"))
        {
            dotCount++;
            if (dotCount > 6)
            {
                Debug.Log($"💥 Ô {gameObject.name} (Yellow) bị sụp do quá 6 chấm!");
                Destroy(gameObject);
                return;
            }

            GenerateDots();
            Debug.Log($"🟡 Ô {gameObject.name} tăng dotCount lên {dotCount}");
            return;
        }

        // 🔴 Nếu là đất Đỏ (Red): lật xúc xắc -> dotCount = 7 - dotCount
        if (CompareTag("Red"))
        {
            dotCount = 7 - dotCount;
            GenerateDots();
            Debug.Log($"🔴 Ô {gameObject.name} đổi mặt xúc xắc -> dotCount mới = {dotCount}");
            return;
        }
    }

    public void OnEnterTile() { }

    // public bool IsBlocked(Vector2Int localPos)
    // {
    //     if (dotCount <= 0) return false;
    //     bool[,] pattern = GetDicePattern(dotCount);
    //     if (localPos.x < 0 || localPos.x > 2 || localPos.y < 0 || localPos.y > 2) return false;
    //     return pattern[localPos.x, localPos.y];
    // }
    public bool IsBlocked(Vector2Int localPos)
    {
        if (dotCount <= 0) return false;
        bool[,] pattern = GetDicePattern(dotCount);

        // Đảo chiều trục Z nếu cần (vì Unity Z+ là phía trước)
        int px = localPos.x;
        int pz = 2 - localPos.y;

        if (px < 0 || px > 2 || pz < 0 || pz > 2) return false;
        return pattern[px, pz];
    }


    private void DecreaseDot()
    {
        if (dotCount > 0)
        {
            dotCount--;
            if (dotCount == 0)
            {
                Debug.Log($"💥 Ô {gameObject.name} bị sụp đổ!");
                Destroy(gameObject);
            }
            else
            {
                GenerateDots();
                Debug.Log($"❄ Ô {gameObject.name} giảm xuống còn {dotCount} chấm");
            }
        }
    }

    private bool[,] GetDicePattern(int dots)
    {
        bool[,] grid = new bool[3, 3];
        switch (dots)
        {
            case 1: grid[1, 1] = true; break;
            case 2: grid[0, 0] = true; grid[2, 2] = true; break;
            case 3: grid[0, 0] = true; grid[1, 1] = true; grid[2, 2] = true; break;
            case 4:
                grid[0, 0] = true; grid[0, 2] = true;
                grid[2, 0] = true; grid[2, 2] = true;
                break;
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

    // 🎲 Tạo vị trí dot theo hướng xúc xắc thật
    private Vector3[] GetDotPositions(int count)
    {
        float o = dotOffset;
        Vector3 P(float x, float z) => new Vector3(x, 0, -z);

        switch (count)
        {
            case 1: return new[] { P(0, 0) };
            case 2: return new[] { P(-o, -o), P(o, o) };
            case 3: return new[] { P(-o, -o), P(0, 0), P(o, o) };
            case 4:
                return new[]
            {
                P(-o, -o), P(o, -o),
                P(-o,  o), P(o,  o)
            };
            case 5:
                return new[]
            {
                P(-o, -o), P(o, -o),
                P(0, 0),
                P(-o,  o), P(o,  o)
            };
            case 6:
                return new[]
            {
                // ✅ xếp DỌC (đúng hướng xúc xắc)
                P(-o, -o), P(-o, 0), P(-o, o),
                P( o, -o), P( o, 0), P( o, o)
            };
            default: return new Vector3[0];
        }
    }
}
