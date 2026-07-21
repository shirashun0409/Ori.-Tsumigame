using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int Width = 8;
    public const int Height = 16;

    [SerializeField]
    private GameObject cellPrefab;

    [SerializeField]
    private float cellSize = 1.0f;

    private void Start()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float offsetX = (Width - 1) * cellSize / 2f;
                float offsetY = (Height - 1) * cellSize / 2f;

                Vector3 position = new Vector3(
                    x * cellSize - offsetX,
                    offsetY - y * cellSize,
                    0);

                Instantiate(cellPrefab, position, Quaternion.identity, transform);
            }
        }
    }
}