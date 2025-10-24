using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [Header("Tham chiếu")]
    public GameObject panelMenu; // Panel chứa các button

    [Header("Các nút level")]
    public Button[] levelButtons; // Gán thủ công trong Inspector

    private void Start()
    {
        // Load dữ liệu trước khi cài đặt nút
        DataManager.Ins.LoadData();

        int unlockedLevel = DataManager.Ins.GetLevel(); // ví dụ: đang ở level 3 -> mở được 1,2,3

        // Thiết lập trạng thái nút
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClick(levelIndex));

            // Chỉ cho bấm nếu level <= unlockedLevel
            bool canPlay = (i + 1) <= unlockedLevel;
            levelButtons[i].interactable = canPlay;

            // Tuỳ chọn: đổi màu nút bị khóa
            ColorBlock colors = levelButtons[i].colors;
            colors.normalColor = canPlay ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            levelButtons[i].colors = colors;
        }
    }

    private void OnLevelButtonClick(int levelIndex)
    {
        Debug.Log($"Load Level {levelIndex + 1}");

        // Lưu lại level hiện tại (nếu muốn)
        DataManager.Ins.SaveLevel(levelIndex + 1);

        // Gọi LevelManager để load level tương ứng
        LevelManager.Instance.LoadLevel(levelIndex);

        // Ẩn panel menu
        if (panelMenu != null)
            panelMenu.SetActive(false);
    }
}
