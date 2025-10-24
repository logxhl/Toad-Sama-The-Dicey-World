using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Gọi hàm mới trong LevelManager
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.CollectCoin(gameObject);
            }

            Destroy(gameObject);
        }
    }
}
