using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateManager : MonoBehaviour
{
    // 가장 바깥의 마나 서클
    private ManaCircle _mainManaCircle;

    // 모든 마나 서클
    private List<ManaCircle> _manaCircles = new List<ManaCircle>();
    private List<InputCircuit> _inputCircuits = new List<InputCircuit>();
    private List<AttributeCircuit> _attributeCircuits = new List<AttributeCircuit>();
    private List<OutputCircuit> _outputCircuits = new List<OutputCircuit>();
    private List<NeutralCircuit> _neutralCircuits = new List<NeutralCircuit>();
    private List<AttributeCircuit> _draggables = new List<AttributeCircuit>();

    // TODO 클리어 여부에 따라서, 자기 자신의 서클들의 색 표시, 회전을 할지 말지 결정함
    // TODO 클릭 감지 콜라이더 필요함
    // TODO 호버링도 감지할 필요성이 있을 것으로 예상됨

    private void Start()
    {
        // 각 스테이지별 자신의 하위 구성요소들 등록
        var gridManager = transform.parent.Find("GridManager");
        if (gridManager == null) return;

        _manaCircles = new List<ManaCircle>();
        _inputCircuits = new List<InputCircuit>();
        _outputCircuits = new List<OutputCircuit>();
        _attributeCircuits = new List<AttributeCircuit>();
        _neutralCircuits = new List<NeutralCircuit>();

        var inputs = gridManager.Find("Inputs");
        if (inputs != null)
        {
            foreach (Transform child in inputs)
            {
                var input = child.GetComponent<InputCircuit>();
                if (input != null) _inputCircuits.Add(input);
            }
        }

        var outputs = gridManager.Find("Outputs");
        if (outputs != null)
        {
            foreach (Transform child in outputs)
            {
                var output = child.GetComponent<OutputCircuit>();
                if (output != null) _outputCircuits.Add(output);
            }
        }

        var dragable = gridManager.Find("Dragable");
        if (dragable != null)
        {
            foreach (Transform child in dragable)
            {
                // 이미 Dragable 의 자식으로 넣어놨으니 bool check는 안해도 됨
                var attribute = child.GetComponent<AttributeCircuit>();
                if (attribute != null) _draggables.Add(attribute);
            }
        }

        var attributes = gridManager.Find("Attributes");
        if (attributes != null)
        {
            foreach (Transform child in attributes)
            {
                var attribute = child.GetComponent<AttributeCircuit>();
                if (attribute != null) _attributeCircuits.Add(attribute);
            }
        }

        var circles = gridManager.Find("Circles");
        if (circles != null)
        {
            foreach (Transform child in circles)
            {
                var mana = child.GetComponent<ManaCircle>();
                if (mana != null) _manaCircles.Add(mana);

                var neutral = child.GetComponent<NeutralCircuit>();
                if (neutral != null) _neutralCircuits.Add(neutral);
            }

            // 가장 긴 것이 main 마나 서클임
            if (_manaCircles.Count > 0)
            {
                _mainManaCircle = _manaCircles.OrderByDescending(c => c.diameter).First();
            }
        }

        Debug.Log($"어 나 {transform.parent.name}인데, ");
        Debug.Log("_mainManaCircle position: " + _mainManaCircle.transform.position);
        Debug.Log($"총 마나 서클 개수는 {_manaCircles.Count}이고, ");
        Debug.Log($"총 입력 회로 개수는 {_inputCircuits.Count}이고, ");
        Debug.Log($"총 속성 회로 개수는 {_attributeCircuits.Count}이고, ");
        Debug.Log($"총 중립 회로 개수는 {_neutralCircuits.Count}이고, ");
        Debug.Log($"총 출력 회로 개수는 {_outputCircuits.Count}이고, ");
        Debug.Log($"이야 수고해 ---------------------------------");
    }

    public void ResetDraggable()
    {
        foreach (var draggable in _draggables)
        {
            draggable.GetComponent<DraggableObject>().ResetPosition();
        }
    }
    
    public void ResetManaCircle()
    {
        foreach (var circle in _manaCircles)
        {
            circle.ResetManaCircle(2);
        }
    }

    // Getter
    public ManaCircle MainCircle => _mainManaCircle;
    public List<ManaCircle> ManaCircles => _manaCircles;
    public List<InputCircuit> InputCircuits => _inputCircuits;
    public List<OutputCircuit> OutputCircuits => _outputCircuits;
    public List<NeutralCircuit> NeutralCircuits => _neutralCircuits;
    public List<AttributeCircuit> AttributeCircuits => _attributeCircuits;
    public List<AttributeCircuit> Draggables => _draggables;
}
