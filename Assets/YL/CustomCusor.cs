using UnityEngine;

public class CustomCursor2D : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Vector2 defaultHotspot = new Vector2(125, 125);



    void Start()
    {
        Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
    }


}
