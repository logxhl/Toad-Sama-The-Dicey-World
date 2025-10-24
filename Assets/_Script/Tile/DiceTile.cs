using UnityEngine;

public class DiceTile : MonoBehaviour
{
    public string tileType; // "Green", "Blue", "Yellow", "Red", "Start"
    public int obstacleCount = 0; // từ 0 đến 6
    public GameObject[] dots; // các chấm hiển thị
    public Material brokenMaterial;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        for (int i = 0; i < dots.Length; i++)
            dots[i].SetActive(i < obstacleCount);
    }

    public void OnPlayerLeave()
    {
        switch (tileType)
        {
            case "Green":
                break;

            case "Blue":
                obstacleCount = Mathf.Max(0, obstacleCount - 1);
                if (obstacleCount == 1)
                    Collapse();
                break;

            case "Yellow":
                obstacleCount++;
                if (obstacleCount >= 6)
                    Collapse();
                break;

            case "Red":
                obstacleCount = 7 - obstacleCount;
                break;
        }

        UpdateVisual();
    }

    private void Collapse()
    {
        if (brokenMaterial)
            rend.material = brokenMaterial;
        else
            gameObject.SetActive(false);
    }
}
