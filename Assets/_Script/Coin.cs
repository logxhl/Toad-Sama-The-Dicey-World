using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // UIManager.Instance.UpdateSproutCount(+1); // cộng 1 sprout
            LevelManager.Instance.CollectCoin(this);
            Destroy(gameObject);
        }
    }
}
