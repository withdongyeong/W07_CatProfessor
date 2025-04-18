using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

[Serializable]
public class StageClearEntry
{
    public string stageName;
    public bool isCleared;

    public StageClearEntry(string stageName, bool isCleared)
    {
        this.stageName = stageName;
        this.isCleared = isCleared;
    }
}

public enum StageStatus
{
    Locked,
    Available,
    Cleared
}


[Serializable]
public class StageClearData
{
    public List<StageClearEntry> clearStatus = new List<StageClearEntry>();
    public bool isEnding = false; // 기본값 false
}

public class StageDataManager : MonoBehaviour
{
    public static StageDataManager Instance { get; private set; }
    private static string savePath;

    private int endingThreshold = 10;
    private StageClearData stageData;
    private bool isEndingCached = false; // 캐싱된 isEnding 값
    private bool isFirstCheck = true; // 최초 체크 여부

    private string[] orderedStageNames = new string[]
    {
        "Stage_1_1", "Stage_1_2", "Stage_1_3", "Stage_1_4", "Stage_1_5", "Stage_1_6",
        "Stage_1_7", "Stage_1_8", "Stage_1_9", "Stage_1_10", "Stage_1_11", "Stage_1_12",
        "Stage_1_13", "Stage_1_14", "Stage_1_15", "Stage_1_16", "Stage_1_17", "Stage_1_18",
        "Stage_1_19", "Stage_1_20", "Stage_1_21", "Stage_1_22"
    };

    public StageStatus GetStageStatus(string stageName)
    {
        int index = Array.IndexOf(orderedStageNames, stageName);
        if (index == -1) return StageStatus.Locked;

        if (IsStageCleared(stageName))
            return StageStatus.Cleared;

        if (index == 0)
            return StageStatus.Available;

        string prevStage = orderedStageNames[index - 1];
        return IsStageCleared(prevStage) ? StageStatus.Available : StageStatus.Locked;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "stageData.json");
            LoadStageData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadStageData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            stageData = JsonUtility.FromJson<StageClearData>(json);
        }
        else
        {
            stageData = new StageClearData();
            SaveStageData();
        }

        if (stageData == null)
            stageData = new StageClearData();

        isEndingCached = stageData.isEnding; // ✅ 파일에서 엔딩 여부 로드하여 캐싱
    }

    public void SaveStageData()
    {
        if (stageData == null)
            stageData = new StageClearData();

        stageData.isEnding = GetClearedStageCount() >= endingThreshold; // ✅ 클리어 개수가 기준을 넘으면 엔딩 활성화
        isEndingCached = stageData.isEnding; // ✅ 메모리에 캐싱

        string json = JsonUtility.ToJson(stageData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Saved At : " + savePath);
    }

    public bool IsStageCleared(string stageName)
    {
        return stageData.clearStatus.Exists(entry => entry.stageName == stageName && entry.isCleared);
    }

    public void SetStageCleared(string stageName)
    {
        if (stageData == null)
            return;

        StageClearEntry existingEntry = stageData.clearStatus.Find(entry => entry.stageName == stageName);
        if (existingEntry == null)
            stageData.clearStatus.Add(new StageClearEntry(stageName, true));
        else
            existingEntry.isCleared = true;

        SaveStageData();
    }

    public bool IsEnding()
    {
        
        if (isFirstCheck) // ✅ 최초 호출 시 파일에서 엔딩 여부 갱신
        {
            isFirstCheck = false;
            LoadStageData();
        }

        if (isEndingCached) {
            Debug.Log("엔딩임!");
        } else {
            Debug.Log("엔딩아님");
        }
        
        return isEndingCached; // ✅ 이후에는 메모리 값 반환
    }

    public void UpdateEndingStatus()
    {
        bool newEndingStatus = GetClearedStageCount() >= endingThreshold;

        if (newEndingStatus != isEndingCached)
        {
            isEndingCached = newEndingStatus;
            SaveStageData(); // ✅ 변경되었을 때만 저장
        }
    }

    public Dictionary<string, bool> GetAllClearStatus()
    {
        Dictionary<string, bool> clearStatusDict = new Dictionary<string, bool>();
        foreach (var entry in stageData.clearStatus)
        {
            clearStatusDict[entry.stageName] = entry.isCleared;
        }
        return clearStatusDict;
    }

    private int GetClearedStageCount()
    {
        return stageData.clearStatus.FindAll(entry => entry.isCleared).Count;
    }
}
