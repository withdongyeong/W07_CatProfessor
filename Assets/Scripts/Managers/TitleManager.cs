using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleManager : MonoBehaviour
{
    public Button triggerButton;
    public Button startButton;
    public Button exitButton;
    public TextMeshProUGUI talkBox;

    private bool triggerActivated = false;

    void Start()
    {
        StageDataManager.Instance.UpdateEndingStatus();
        if (StageDataManager.Instance.IsEnding())
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.Greeting_Ending);
        }
        else
        {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.Greeting);
        }

        startButton.interactable = false;
        exitButton.interactable = false;

        triggerButton.onClick.AddListener(OnTriggerButtonClick);
        startButton.onClick.AddListener(OnStartButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);

        AddButtonEvents(startButton);
        AddButtonEvents(exitButton);

        Professor.Instance.SetAnimation(Professor.AnimationType.Idle);
    }

    void OnTriggerButtonClick()
    {
        triggerActivated = true;
        triggerButton.gameObject.SetActive(false);

        Professor.Instance.SayRandom(ScriptManager.ScriptCategory.TitleTrigger);
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "과제 하러가기";
        exitButton.GetComponentInChildren<TextMeshProUGUI>().text = "조금만 쉬러가기";

        startButton.interactable = true;
        exitButton.interactable = true;
    }

    void OnStartButtonClick()
    {
        SceneManager.LoadScene("StageSelect");
    }

    void OnExitButtonClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void AddButtonEvents(Button button)
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

    void OnMouseEnterButton()
    {
        if (triggerActivated) return;
        Professor.Instance.SayRandom(ScriptManager.ScriptCategory.TitleTriggerStop);
    }
}
