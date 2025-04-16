using UnityEngine;
using System.Collections.Generic;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager Instance { get; private set; }

    private Dictionary<ScriptCategory, List<string>> scriptDictionary = new Dictionary<ScriptCategory, List<string>>();

    public enum ScriptCategory
    {
        TitleTrigger,
        TitleTriggerStop,
        Greeting,
        StageSelect,
        GameStart,
        GameWin,
        WaitMana,
        Greeting_Ending,
        StageSelect_Ending,
        GameStart_Ending,
        GameWin_Ending,
        Compliment
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeScripts();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeScripts()
    {
        scriptDictionary[ScriptCategory.TitleTrigger] = new List<string> {
            "훌륭한 선택이다냥!",
            "오늘은 마법을 잔뜩 연구할 거다냥!",
            "자네가 배울 것이 아주 많다냥!"
        };
        scriptDictionary[ScriptCategory.TitleTriggerStop] = new List<string> {
            "앗, 그러면 안 된다냥!",
            "어허, 잠깐 기다려봐냥!",
            "그건 곤란하다냥!"
        };

        scriptDictionary[ScriptCategory.Greeting] = new List<string> {
            "환영한다냥!",
            "같이 마법 연구하자냥!",
            "오늘도 마법 실험이다냥!"
        };
        scriptDictionary[ScriptCategory.StageSelect] = new List<string> {
            "이번엔 어떤 과제를 선택할 거냥?",
            "자네의 선택이 기대된다냥!",
            "좋아! 연구를 시작해보자냥!"
        };
        scriptDictionary[ScriptCategory.GameStart] = new List<string> {
            "이 마법진, 쉽지 않을 거다냥!",
            "아주 흥미로운 마법 회로다냥!",
            "자네를 위해 준비한 과제다냥!"
        };
        scriptDictionary[ScriptCategory.GameWin] = new List<string> {
            "오오, 자네 재능이 대단하다냥!",
            "나랑 오래도록 연구하면 좋겠다냥!",
            "자네의 아이디어가 혁신적이다냥!"
        };

        scriptDictionary[ScriptCategory.WaitMana] = new List<string> {
            "마나가 아직 흐르고 있다냥!",
            "조금 더 기다리면 마나가 안정될 거다냥!"
        };

        scriptDictionary[ScriptCategory.Greeting_Ending] = new List<string> {
            "드디어 찾아왔다냥!",
            "자네를 기다리고 있었다냥!",
            "반갑다냥! 다시 만날 줄 알았다냥!"
        };
        scriptDictionary[ScriptCategory.StageSelect_Ending] = new List<string> {
            "자네의 마법 탐구심은 정말 대단하다냥!",
            "이제 거의 일류 마법사의 경지에 다다랐다냥!"
        };
        scriptDictionary[ScriptCategory.GameStart_Ending] = new List<string> {
            "이번엔 어떤 기발한 해법을 보여줄 거냥?",
            "아, 자네의 도전을 기다리고 있었다냥!"
        };
        scriptDictionary[ScriptCategory.GameWin_Ending] = new List<string> {
            "자네의 마법 이해력은 정말 대단하다냥!",
            "나랑 함께 위대한 마법사가 될 수 있을 거다냥!"
        };
        scriptDictionary[ScriptCategory.Compliment] = new List<string> {
            "자네, 재능있다냥!!",
            "자네의 끈질긴 탐구에 아낌없는 찬사를 보낸다냥!!",
            "탐구하는 자네가 너무나 사랑스럽다냥!!"
        };
    }

    public string Get(ScriptCategory category)
    {
        if (scriptDictionary.ContainsKey(category))
        {
            List<string> scripts = scriptDictionary[category];
            return scripts[Random.Range(0, scripts.Count)];
        }
        return "⚠ No script found for this category.";
    }
}
