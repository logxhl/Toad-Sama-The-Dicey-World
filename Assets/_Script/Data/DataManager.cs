using UnityEngine;
using System;
public class DataManager : MonoBehaviour
{
    public static DataManager Ins;                          // Singleton instance
    public bool isLoaded = false;                           // Đã load data chưa
    public GameSave gameSave;                               // Data game hiện tại
    public GameSave gameSave_BackUp;                        // Backup data
    // Load data

    private void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadData(); // Tự load dữ liệu khi game khởi động
    }

    public void LoadData()
    {
        if (isLoaded == false)
        {
            if (PlayerPrefs.HasKey("GameSave"))
                gameSave = JsonUtility.FromJson<GameSave>(PlayerPrefs.GetString("GameSave"));
            if (gameSave.isNew)
            {
                InitData();
            }
            isLoaded = true;
        }
    }

    // Lưu game
    public void SaveGame()
    {
        try
        {
            if (!isLoaded)
                return;
            if (gameSave == null)
            {
                if (gameSave_BackUp != null)
                {
                    gameSave = gameSave_BackUp;
                    Debug.LogError("gameSave bị null , backup thành công ");
                }
                else
                {
                    InitData();
                    Debug.LogError("gameSave bị null , backup không thành công . Reset data");
                }
            }
            gameSave_BackUp = gameSave;
            PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(gameSave));
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi LoadData:" + ex);
        }
    }
    // Khởi tạo data mới
    void InitData()
    {
        gameSave = new GameSave();
        gameSave.isNew = false;
    }
    // Lấy level hiện tại
    public int GetLevel()
    {
        return gameSave.level;
    }

    // Lưu level
    public void SaveLevel(int level)
    {
        gameSave.level = level;
        SaveGame();
    }
    // Lấy ID level
    public int GetLevelId()
    {
        int levelCur = GetLevel();
        return levelCur;
    }
    // Reset data khi reset component
    void Reset()
    {
        gameSave.isNew = true;
    }
}
