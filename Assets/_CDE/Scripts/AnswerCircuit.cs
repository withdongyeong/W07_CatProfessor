using UnityEngine;

public class AnswerCircuit : MonoBehaviour
{
    public ManaProperties.ManaType AnswerType;
    public Vector2 AnswerPos { get; private set; }
    public bool IsAnswer  { get; private set; }

    private SpriteRenderer _renderer;

    private void Start()
    {
        AnswerPos = transform.position;
        
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.enabled = false;
    }

    public void ChangeAnswer(bool isAnswer)
    {
        IsAnswer = isAnswer;
    }
}
