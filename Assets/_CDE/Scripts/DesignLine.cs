using UnityEngine;

public class DesignLine : MonoBehaviour
{
    private bool _isHighlighted;
    private SpriteRenderer _renderer;
    
    private int activeOrbiters = 2;
    private float orbitSpeed = 30f;
    private bool isRotating = true;
    private bool isHighlighted = false;
    private float highlightPulseSpeed = 2f;
    private float highlightIntensity = 0.4f;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.color = Color.white;
        
        SetHighlight(false);
    }

    private void Update()
    {
        if (!_isHighlighted)
        {
            return;
        }
        
        float pulse = Mathf.PingPong(Time.time * highlightPulseSpeed, 1f); // 0~1 사이로 반복
        float lerpAlpha = Mathf.Lerp(0.1f, 1f, pulse); // 🔥 최소~최대 알파 값 지정 (ex. 30%~100%)

        Color currentColor = _renderer.color;
        currentColor.a = lerpAlpha;
        _renderer.color = currentColor;
    }

    public void SetHighlight(bool highlight)
    {
        _isHighlighted = highlight;

        if (!_isHighlighted)
        {
            _renderer.color = Color.white;
        }
    }
}