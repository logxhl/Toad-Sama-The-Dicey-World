using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class TileMaterialData
{
    public string tag;          // Tag của ô (Blue, Red, Yellow, Green...)
    public Material material;   // Material tương ứng
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Prefab và Gốc Map")]
    public GameObject tilePrefab;     // Prefab TerrainTile
    public Transform tileParent;      // Nơi chứa các ô đất
    public GameObject coinPrefab;

    [Header("Danh sách Level (ScriptableObject)")]
    public List<LevelDataAsset> levels = new List<LevelDataAsset>();

    [Header("Material cho từng loại ô đất")]
    public List<TileMaterialData> tileMaterials = new List<TileMaterialData>();

    [Header("UI")]
    public TextMeshProUGUI coinCounterText;
    public TextMeshProUGUI levelText;
    public Button restartLevel;

    [Header("Player reference (kéo thả)")]
    public GridPlayerMovement player;

    // runtime
    private List<GameObject> spawnedTiles = new List<GameObject>();
    private List<GameObject> spawnedCoins = new List<GameObject>();
    private int currentLevelIndex = 0;
    private int totalCoins = 0;
    private int coinsCollected = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (restartLevel != null)
            restartLevel.onClick.AddListener(RestartLevel);

    }

    // ===== Register coin khi spawn (nếu TerrainTile spawn coin tự động, có thể gọi RegisterCoin)
    public void RegisterCoin(GameObject coin)
    {
        if (!spawnedCoins.Contains(coin))
            spawnedCoins.Add(coin);
    }

    // Coin được ăn: coinScript nên gọi LevelManager.Instance.CollectCoin(gameObject)
    public void CollectCoin(GameObject coinObj)
    {
        if (spawnedCoins.Contains(coinObj))
            spawnedCoins.Remove(coinObj);

        coinsCollected++;
        int remaining = Mathf.Max(0, totalCoins - coinsCollected);
        if (coinCounterText != null)
            coinCounterText.text = remaining.ToString();

        if (spawnedCoins.Count == 0 || remaining <= 0)
        {
            Debug.Log("All coins collected -> Next level");
            NextLevel();
        }
    }

    // ===== Load level
    public void LoadLevel(int index)
    {
        if (levels == null || levels.Count == 0)
        {
            Debug.LogError("No levels assigned in LevelManager!");
            return;
        }

        if (index < 0 || index >= levels.Count)
        {
            Debug.LogError($"Level index {index} out of range");
            return;
        }

        // Clear old
        ClearCurrentLevel();

        LevelDataAsset data = levels[index];

        // Spawn tiles
        foreach (var t in data.tiles)
        {
            Vector3 pos = new Vector3(t.position.x, 0f, t.position.y);
            GameObject newTile = Instantiate(tilePrefab, pos, Quaternion.identity, tileParent);
            // nếu prefab yêu cầu offset y, set localPosition.y
            Vector3 lp = newTile.transform.localPosition;
            newTile.transform.localPosition = new Vector3(lp.x, 1.05f, lp.z);

            newTile.tag = t.tag;
            TerrainTile terrain = newTile.GetComponent<TerrainTile>();
            if (terrain != null)
            {
                terrain.dotCount = t.dotCount;
                terrain.GenerateDots();
            }

            ApplyMaterialByTag(newTile, t.tag);
            spawnedTiles.Add(newTile);
        }

        // Spawn coins by absolute positions defined in LevelDataAsset (Vector2Int)
        foreach (var coinPos in data.coinPositions)
        {
            Vector3 pos = new Vector3(coinPos.x, 1.2f, coinPos.y);
            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity, tileParent);
            spawnedCoins.Add(coin);
        }

        totalCoins = data.coinPositions != null ? data.coinPositions.Count : spawnedCoins.Count;
        coinsCollected = 0;

        // Update UI
        if (coinCounterText != null) coinCounterText.text = totalCoins.ToString();
        if (levelText != null) levelText.text = data.levelName;

        currentLevelIndex = index;

        // Spawn player at start pos but WAIT one frame to ensure tiles present
        StartCoroutine(PlacePlayerNextFrame(data.playerStartPos));
    }

    private IEnumerator PlacePlayerNextFrame(Vector2Int startGrid)
    {
        // Đợi frame cho Instantiate hoàn tất
        yield return new WaitForEndOfFrame();

        if (player == null)
            player = FindObjectOfType<GridPlayerMovement>();

        if (player == null)
        {
            Debug.LogError("Player not found for LevelManager.PlacePlayerNextFrame");
            yield break;
        }

        // Dừng mọi chuyển động hiện tại của player
        player.StopMovementCoroutines();

        Vector3 startPos = new Vector3(startGrid.x, 2f, startGrid.y);
        // gọi public method để đặt player và sync nội bộ của player
        player.PlaceAt(startPos);
    }

    private void ApplyMaterialByTag(GameObject tileObj, string tag)
    {
        Renderer rend = tileObj.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning($"Tile {tileObj.name} không có Renderer để gán material!");
            return;
        }

        foreach (var m in tileMaterials)
        {
            if (m.tag == tag && m.material != null)
            {
                rend.material = m.material;
                return;
            }
        }

        rend.material.color = Color.gray;
    }

    public void ClearCurrentLevel()
    {
        // Destroy tiles
        foreach (var t in spawnedTiles)
            if (t != null) Destroy(t);
        spawnedTiles.Clear();

        // Destroy coins
        foreach (var c in spawnedCoins)
            if (c != null) Destroy(c);
        spawnedCoins.Clear();

        totalCoins = 0;
        coinsCollected = 0;

        if (coinCounterText != null) coinCounterText.text = "0";
    }

    public void NextLevel()
    {
        int nextIndex = (currentLevelIndex + 1) % levels.Count;

        // Save if you use DataManager:
        if (DataManager.Ins != null && nextIndex + 1 > DataManager.Ins.GetLevel())
        {
            DataManager.Ins.SaveLevel(nextIndex + 1);
            Debug.Log($"Saved new unlocked level {nextIndex + 1}");
        }

        LoadLevel(nextIndex);
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }
}
