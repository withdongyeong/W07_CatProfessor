using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class ManaCircleAnswser
{
    public ManaProperties.ManaType type;
    public int answerCount;
    public bool isCorrect;
}

public class HintManager : MonoBehaviour
{
    public Action<ManaProperties.ManaType, Vector2> OnCheckPositionAction;
    private List<AnswerCircuit> _answerCircuits = new List<AnswerCircuit>();

    public Action<ManaProperties.ManaType, int> OnCheckCircleAction;
    [SerializeField] private List<ManaCircleAnswser> manaCircleAnswserList;
    
    // Getter
    public List<AnswerCircuit> AnswerCircuits => _answerCircuits;
    public List<ManaCircleAnswser> ManaCircleAnswsers => manaCircleAnswserList;

    private void Start()
    {
        // 자식 Answer Circuit 가져오기 
        for (int i = 0; i < transform.childCount; i++)
        {
            var answer = transform.GetChild(i).GetComponent<AnswerCircuit>();
            if (answer != null)
            {
                _answerCircuits.Add(answer);
            }
        }
        
        // Draggable의 OnMouseUp 시 액션 실행
        OnCheckPositionAction += CheckPosition;
        OnCheckCircleAction += CheckCircle;
    }

    private void CheckPosition(ManaProperties.ManaType type, Vector2 position)
    {
        // 정답 체크
        foreach (var answer in _answerCircuits)
        {
            if (type != answer.AnswerType)
            {
                continue;
            }
            
            answer.ChangeAnswer(position == answer.AnswerPos);
        }
    }
    
    private void CheckCircle(ManaProperties.ManaType type, int count)
    {
        foreach (var circle in manaCircleAnswserList)
        {
            if (type != circle.type)
            {
                continue;
            }

            circle.isCorrect = count == circle.answerCount;
        }
    }
    
    private float CalculateCorrectRate()
    {
        var correctCount = _answerCircuits.Count(a => a.IsAnswer);
        return (float)correctCount / _answerCircuits.Count;
    }
}
