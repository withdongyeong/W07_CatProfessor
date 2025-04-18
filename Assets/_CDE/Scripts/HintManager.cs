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
        
        ValidateAnswerData();
        
        // Draggable의 OnMouseUp 시 액션 실행
        OnCheckPositionAction += CheckPosition;
        OnCheckCircleAction += CheckCircle;
    }
    
    private void ValidateAnswerData()
    {
        string stageName = GetComponentInParent<StageRootMarker>()?.gameObject.name ?? "(알 수 없음)";

        foreach (var answer in ManaCircleAnswsers)
        {
            if (answer.type == ManaProperties.ManaType.None)
            {
                Debug.LogWarning($"[HintManager] ({stageName}) None 타입의 마나서클 정답이 설정되어 있습니다.");
            }
        }

        foreach (var circuit in AnswerCircuits)
        {
            if (circuit.AnswerType == ManaProperties.ManaType.None)
            {
                Debug.LogWarning($"[HintManager] ({stageName}) None 타입의 회로 정답이 설정되어 있습니다. ({circuit.name})");
            }
        }
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
