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
        if (index < 0 || index >= levels.Count)
        {
            Debug.LogError($"❌ Level index {index} không tồn tại!");
            return;
        }

        ClearCurrentLevel();

        LevelDataAsset data = levels[index];
        Debug.Log($"🔹 Đang load: {data.levelName}");

        foreach (var tile in data.tiles)
        {
            Vector3 pos = new Vector3(tile.position.x, 0f, tile.position.y);
            GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
            newTile.transform.localPosition = new Vector3(newTile.transform.localPosition.x, 1.05f, newTile.transform.localPosition.z);

            // Cài tag, dotCount
            newTile.tag = tile.tag;
            TerrainTile terrain = newTile.GetComponent<TerrainTile>();
            if (terrain)
            {
                terrain.dotCount = tile.dotCount;
                terrain.GenerateDots();
            }

            // 🔹 Gán material tương ứng theo tag
            ApplyMaterialByTag(newTile, tile.tag);

            spawnedTiles.Add(newTile);
            if (tile.hasCoin && terrain.coinPrefab != null)
            {
                terrain.coinPositions.Add(tile.coinLocalPos);
                terrain.GenerateCoins();
            }

        }
        allCoins.Clear();
        collectedCoins = 0;


        currentLevelIndex = index;

        // Spawn player tại ô có tag "Start"
        GameObject startTile = GameObject.FindGameObjectWithTag("Start");
        if (startTile != null)
        {
            var player = FindObjectOfType<GridPlayerMovement>();
            if (player != null)
            {
                Vector3 pos = startTile.transform.position;
                pos.y += 2f;
                player.transform.position = pos;
                player.ForceSyncPosition();
                Debug.Log($"✅ Player spawn tại {pos}");
            }
        }
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
