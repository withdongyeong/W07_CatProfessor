using UnityEngine;
using UnityEngine.EventSystems;

public class SubmitBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isHolding = false;
    private bool _previousIsHolding = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHolding = false;
    }

    private void Update()
    {
        if (_isHolding == _previousIsHolding) return;
        
        GameManager.Instance.Submit(_isHolding);
        _previousIsHolding = _isHolding;
    }
}
