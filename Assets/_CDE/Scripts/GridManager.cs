using UnityEngine;

public class GridManager : MonoBehaviour
{
    public enum GridType
    {
        Cross,
        Dot,
    }

    [SerializeField] private GridType gridType;
    [SerializeField] private Material crossM;
    [SerializeField] private Material dotM;
    private SpriteRenderer _renderer;
    
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        transform.SetParent(Camera.main.transform);
        
        ChangeGridType(gridType);
        ActivateGrid(false);
    }

    private void ChangeGridType(GridType type)
    {
        // Grid Material 변경
        switch (type)
        {
            case GridType.Cross:
                _renderer.material = crossM;
                break;
            case GridType.Dot:
                _renderer.material = dotM;
                break;
        }
    }

    private void ActivateGrid(bool isActive)
    {
        // Grid 활성화
        _renderer.enabled = isActive;
    }
}
