using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button triggerBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button exitBtn;
    
    [SerializeField] private TMP_Text startText;
    [SerializeField] private TMP_Text exitText;
    
    private bool _isTriggerActivated = false;

    public void Start()
    {
        StageDataManager.Instance.UpdateEndingStatus();
        Professor.Instance.SayRandom(StageDataManager.Instance.IsEnding()
            ? ScriptManager.ScriptCategory.Greeting_Ending
            : ScriptManager.ScriptCategory.Greeting);

        triggerBtn.onClick.AddListener(OnClickTriggerBtn);
        startBtn.onClick.AddListener(OnClickStartBtn);
        exitBtn.onClick.AddListener(OnClickExitBtn);
        
        AddButtonEvents(startBtn);
        AddButtonEvents(exitBtn);

        startBtn.interactable = false;
        exitBtn.interactable = false;
        
        Professor.Instance.SetAnimation(Professor.AnimationType.Idle);
    }
    
    private void AddButtonEvents(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };

        entry.callback.AddListener((data) => OnMouseEnterButton());

        trigger.triggers.Add(entry);
    }
    
    private void OnMouseEnterButton()
    {
        if (_isTriggerActivated) return;
        Professor.Instance.SayRandom(ScriptManager.ScriptCategory.TitleTriggerStop);
    }
    
    private void OnClickStartBtn()
    {
        GameManager.Instance.CurrentGameState = GameManager.gameState.StageSelecting;
    }
    
    private void OnClickTriggerBtn()
    {
        _isTriggerActivated = true;
        triggerBtn.interactable = false;

        Professor.Instance.SayRandom(ScriptManager.ScriptCategory.TitleTrigger);
        startText.text = "과제 하러가기";
        exitText.text = "조금만 쉬러가기";

        startBtn.interactable = true;
        exitBtn.interactable = true;
    }

    private void OnClickExitBtn()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
