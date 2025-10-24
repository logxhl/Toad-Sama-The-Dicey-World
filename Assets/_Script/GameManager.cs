using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Button btnMenu;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (btnMenu != null)
        {
            btnMenu.onClick.AddListener(LoadScene);
        }
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}
