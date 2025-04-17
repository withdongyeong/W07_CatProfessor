using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HintManager : MonoBehaviour
{
    public Action<ManaProperties.ManaType, Vector2> OnCheckAnswerAction;
    private List<AnswerCircuit> _answerCircuits = new List<AnswerCircuit>();

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
        OnCheckAnswerAction += CheckAnswer;
    }

    private void CheckAnswer(ManaProperties.ManaType type, Vector2 position)
    {
        // 정답 체크
        foreach (var answer in _answerCircuits)
        {
            if (type != answer.AnswerType)
            {
                continue;
            }
            
            answer.ChangeAnswer(position == answer.AnswerPos);
            Debug.Log($"정답 비율: {CalculateCorrectRate()}%");
        }
    }

    private float CalculateCorrectRate()
    {
        var correctCount = _answerCircuits.Count(a => a.IsAnswer);
        return (float)correctCount / _answerCircuits.Count;
    }
}
