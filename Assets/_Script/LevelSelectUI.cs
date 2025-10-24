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
        // Gán sự kiện click cho từng nút
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i; // cần biến tạm để tránh closure bug
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClick(levelIndex));
        }
    }

    private void OnLevelButtonClick(int levelIndex)
    {
        Debug.Log($"Load Level {levelIndex + 1}");

        // Gọi LevelManager để load level tương ứng
        LevelManager.Instance.LoadLevel(levelIndex);

        // Ẩn panel menu sau khi chọn level
        if (panelMenu != null)
            panelMenu.SetActive(false);
    }
}
