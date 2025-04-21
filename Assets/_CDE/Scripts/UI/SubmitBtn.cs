using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubmitBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isHolding = false;
    private bool _previousIsHolding = false;
    [SerializeField] private GameObject mouseImg;
    private List<DesignLine> _designLineList = new List<DesignLine>();

    private void Start()
    {
        DesignLine[] designLines = FindObjectsByType<DesignLine>(FindObjectsSortMode.None);
        _designLineList = new List<DesignLine>(designLines);
    }

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
        mouseImg.SetActive(!_isHolding);
        _previousIsHolding = _isHolding;

        if (_isHolding)
        {
            Professor.Instance.SetAnimation(Professor.AnimationType.AfterEnding);
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.AfterEnding);
            ActivateDesignLines(true);
        }
        else
        {
            Professor.Instance.SetAnimation(Professor.AnimationType.Idle);
            ActivateDesignLines(false);
        }
    }

    private void ActivateDesignLines(bool isActive)
    {
        foreach (var line in _designLineList)
        {
            line.SetHighlight(isActive);
        }
    }
}
