using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StageSelectManager : MonoBehaviour
{
    public GameObject stageButtonPrefab;
    public Transform buttonContainer;
    private const string StageKeyPrefix = "StageCleared_";
    private List<GameObject> stageButtons = new List<GameObject>();

    public int maxStages = 3;
    public int maxLevels = 20;

    void Start()
    {
        Time.timeScale = 1f;

        if (stageButtonPrefab == null)
        {
            Debug.LogError("StageButton 프리팹이 연결되지 않았습니다!");
            return;
        }

        if (buttonContainer == null)
        {
            Debug.LogError("buttonContainer가 설정되지 않았습니다!");
            return;
        }

        GenerateStageButtons(maxStages, maxLevels);
        
        StageDataManager.Instance.UpdateEndingStatus();

        if (StageDataManager.Instance.IsEnding()) {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.StageSelect_Ending);
        } else {
            Professor.Instance.SayRandom(ScriptManager.ScriptCategory.StageSelect);
        }
        
    }

    void GenerateStageButtons(int maxStages, int maxLevels)
    {
        for (int stage = 1; stage <= maxStages; stage++)
        {
            for (int level = 1; level <= maxLevels; level++)
            {
                string sceneName = $"Stage_{stage:D2}_{level:D2}";

                if (!SceneExists(sceneName))
                {
                    continue;
                }

                GameObject newButton = Instantiate(stageButtonPrefab, buttonContainer);
                newButton.name = $"StageButton_{stage}_{level}";

                TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = $"{stage}-{level}";
                }

                string stageKey = $"Stage_{stage:D2}_{level:D2}";
                string prevStageKey = (level == 1) ? $"Stage_{(stage - 1):D2}_{maxLevels:D2}" : $"Stage_{stage:D2}_{(level - 1):D2}";

                bool isCleared = StageDataManager.Instance.IsStageCleared(stageKey);
                bool isPrevCleared = StageDataManager.Instance.IsStageCleared(prevStageKey);

                newButton.SetActive(stage == 1 && level == 1 || isCleared || isPrevCleared);

                // ✅ 클리어한 스테이지면 Check 오브젝트 활성화
                Transform checkObj = newButton.transform.Find("Check");
                if (checkObj != null)
                {
                    checkObj.gameObject.SetActive(isCleared);
                }
                else
                {
                    Debug.LogWarning($"'{newButton.name}' 버튼에서 'Check' 오브젝트를 찾을 수 없습니다.");
                }

                int stageNum = stage;
                int levelNum = level;
                newButton.GetComponentInChildren<Button>().onClick.AddListener(() => LoadStage(stageNum, levelNum));

                stageButtons.Add(newButton);
            }
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer.GetComponent<RectTransform>());
    }


    void LoadStage(int stage, int level)
    {
        string sceneName = $"Stage_{stage:D2}_{level:D2}";
        if (SceneExists(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"씬 {sceneName}이 존재하지 않습니다! Unity Build Settings에서 추가했는지 확인하세요.");
        }
    }

    bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneFileName == sceneName)
                return true;
        }
        return false;
    }
}
