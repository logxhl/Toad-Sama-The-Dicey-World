using UnityEngine;

[ExecuteInEditMode]
public class GridGizmoDrawer : MonoBehaviour
{
    public int gridSize = 30;
    public float cellSize = 1f;
    public Color lineColor = Color.white;

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        float half = gridSize * cellSize / 2f;

        for (int x = 0; x <= gridSize; x++)
        {
            float worldX = -half + x * cellSize;
            Gizmos.DrawLine(
                new Vector3(worldX, 0.01f, -half),
                new Vector3(worldX, 0.01f, half)
            );
        }

        for (int z = 0; z <= gridSize; z++)
        {
            float worldZ = -half + z * cellSize;
            Gizmos.DrawLine(
                new Vector3(-half, 0.01f, worldZ),
                new Vector3(half, 0.01f, worldZ)
            );
        }
    }
}
