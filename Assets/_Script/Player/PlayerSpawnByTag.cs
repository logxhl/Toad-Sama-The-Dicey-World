using UnityEngine;

public class PlayerSpawnByTag : MonoBehaviour
{
    [Header("Spawn Settings")]
    public string startTag = "Start";
    public float heightOffset = 2f;

    void Start()
    {
        GameObject startLand = GameObject.FindGameObjectWithTag(startTag);

        if (startLand != null)
        {
            Vector3 pos = startLand.transform.position;
            pos.y += heightOffset;
            transform.position = pos;

            Debug.Log($"Player spawned on land with tag '{startTag}' at {pos}");

            // Cập nhật lại gridPos của player
            GridPlayerMovement gridMove = GetComponent<GridPlayerMovement>();
            if (gridMove != null)
            {
                gridMove.ForceSyncPosition();
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy vùng đất có tag '{startTag}'!");
        }
    }
}
