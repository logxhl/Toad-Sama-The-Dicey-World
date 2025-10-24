using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileMaterialData
{
    public string tag;          // Tag của ô (Blue, Red, Yellow, Green, Start)
    public Material material;   // Material tương ứng
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Prefab và Gốc Map")]
    public GameObject tilePrefab;     // Prefab TerrainTile
    public Transform tileParent;      // Nơi chứa các ô đất

    [Header("Danh sách Level (ScriptableObject)")]
    public List<LevelDataAsset> levels = new List<LevelDataAsset>();

    [Header("Material cho từng loại ô đất")]
    public List<TileMaterialData> tileMaterials = new List<TileMaterialData>();

    private List<GameObject> spawnedTiles = new List<GameObject>();
    private int currentLevelIndex = 0;
    private List<GameObject> allCoins = new List<GameObject>();
    private int collectedCoins = 0;
    public GameObject coinPrefab;
    private List<GameObject> spawnedCoins = new();
    private int totalCoins;
    private int coinsCollected;


    public void OnCoinCollected(Coin coin)
    {
        coinsCollected++;

        if (coinsCollected >= totalCoins)
        {
            Debug.Log("🎉 Collected all coins! Loading next level...");
            NextLevel();
        }
    }
    public void RegisterCoin(GameObject coin)
    {
        allCoins.Add(coin);
    }

    public void CollectCoin(Coin coin)
    {
        collectedCoins++;
        allCoins.Remove(coin.gameObject);

        if (allCoins.Count == 0)
        {
            Debug.Log("🎉 Ăn hết coin -> Next Level!");
            NextLevel();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLevel(0); // load level đầu tiên
    }


    public void LoadLevel(int index)
    {
        // Clear cũ
        ClearCurrentLevel();

        LevelDataAsset data = levels[index];

        // Spawn ô đất
        foreach (var tile in data.tiles)
        {
            Vector3 pos = new Vector3(tile.position.x, 0f, tile.position.y);
            GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
            newTile.transform.localPosition = new Vector3(newTile.transform.localPosition.x, 1.05f, newTile.transform.localPosition.z);

            // Set màu/tag/dot...
            newTile.tag = tile.tag;
            TerrainTile terrain = newTile.GetComponent<TerrainTile>();
            if (terrain)
            {
                terrain.dotCount = tile.dotCount;
                terrain.GenerateDots();
            }
            // Áp dụng material dựa trên tag
            ApplyMaterialByTag(newTile, tile.tag);
            spawnedTiles.Add(newTile);
        }

        // ✅ Spawn coin theo danh sách riêng
        foreach (var coinPos in data.coinPositions)
        {
            Vector3 pos = new Vector3(coinPos.x, 1.5f, coinPos.y);
            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity, tileParent);
            spawnedCoins.Add(coin);
        }
        totalCoins = data.coinPositions.Count;
        coinsCollected = 0;

    }

    private void ApplyMaterialByTag(GameObject tileObj, string tag)
    {
        Renderer rend = tileObj.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning($"⚠ Tile {tileObj.name} không có Renderer để gán material!");
            return;
        }

        // Tìm trong danh sách
        foreach (var data in tileMaterials)
        {
            if (data.tag == tag && data.material != null)
            {
                rend.material = data.material;
                return;
            }
        }

        // Nếu không tìm thấy tag tương ứng → gán màu mặc định
        rend.material.color = Color.gray;
    }

    public void ClearCurrentLevel()
    {
        foreach (var tile in spawnedTiles)
        {
            if (tile != null)
                Destroy(tile);
        }
        spawnedTiles.Clear();
    }

    public void NextLevel()
    {
        int nextIndex = (currentLevelIndex + 1) % levels.Count;
        LoadLevel(nextIndex);
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }
}
