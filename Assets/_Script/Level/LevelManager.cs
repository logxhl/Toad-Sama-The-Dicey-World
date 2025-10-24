using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Prefab và Gốc Map")]
    public GameObject tilePrefab;     // prefab TerrainTile
    public Transform tileParent;      // cha chứa các ô

    [Header("Danh sách Level (ScriptableObject)")]
    public List<LevelDataAsset> levels = new List<LevelDataAsset>();

    private List<GameObject> spawnedTiles = new List<GameObject>();
    private int currentLevelIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLevel(0); // load level đầu tiên khi bắt đầu
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

            spawnedTiles.Add(newTile);
        }

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
