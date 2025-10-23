using UnityEngine;

public class PlayerSpawnByTag : MonoBehaviour
{
    [Header("Spawn Settings")]
    public string startTag = "Start";  // Tag của vùng đất để spawn lên
    public float heightOffset = 2f;  // Độ cao so với mặt đất

    void Start()
    {
        // Tìm vùng đất có tag "Start"
        GameObject startLand = GameObject.FindGameObjectWithTag(startTag);

        if (startLand != null)
        {
            // Lấy vị trí trung tâm vùng đất
            Vector3 pos = startLand.transform.position;
            pos.y += heightOffset;

            // Đặt player tại đó
            transform.position = pos;

            Debug.Log($"✅ Player spawned on land with tag '{startTag}' at {pos}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Không tìm thấy vùng đất có tag '{startTag}'! Player sẽ giữ vị trí hiện tại.");
        }
    }
}
