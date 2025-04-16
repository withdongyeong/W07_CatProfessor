using UnityEngine;

public class GridPoint : MonoBehaviour
{
    public GameObject prefab;
    public Vector2 centerPoint;
    public int xSize = 42;
    public int ySize = 22;
    public int gridSize = 1;

    private void Start()
    {
        CreateDots();
    }

    private void CreateDots()
    {
        var xInitPos = (int)(centerPoint.x - (xSize * 0.5f));
        var xEndPos = (int)(centerPoint.x + (xSize * 0.5f));
        var yInitPos = (int)(centerPoint.y - (ySize * 0.5f));
        var yEndPos = (int)(centerPoint.y + (ySize * 0.5f));
        Debug.Log($"{xInitPos} {xEndPos} {yInitPos} {yEndPos}");

        for (var x = xInitPos; x < xEndPos; x += gridSize)
        {
            for (var y = yInitPos; y < yEndPos; y += gridSize)
            {
                Vector3 pos = new Vector3(x, y ,0);
                Instantiate(prefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
