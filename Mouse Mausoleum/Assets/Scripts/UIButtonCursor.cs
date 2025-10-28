using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // When the mouse enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // shows OS hand
    }

    // When the mouse exits the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // back to default
    }
}
