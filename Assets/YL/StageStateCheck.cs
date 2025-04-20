using System.Collections.Generic;
using UnityEngine;

public class StageStateCheck : MonoBehaviour
{
    // 구간별 스테이지 매핑
    private readonly Dictionary<int, List<string>> stageSections = new Dictionary<int, List<string>>
    {
        { 0, new List<string> { "Stage_1_1", "Stage_1_2", "Stage_1_3", "Stage_1_4" } },
        { 1, new List<string> { "Stage_1_Sub1", "Stage_1_Sub2" } },
        { 2, new List<string> { "Stage_2_1", "Stage_2_2", "Stage_2_3" } },
        { 3, new List<string> { "Stage_2_Sub1", "Stage_2_Sub2" } },
        { 4, new List<string> { "Stage_3_1", "Stage_3_2", "Stage_3_3" } },
        { 5, new List<string> { "Stage_3_Sub1", "Stage_3_Sub2" } },
        { 6, new List<string> { "Stage_4_1", "Stage_4_2", "Stage_4_3", "Stage_4_4" } },
        { 7, new List<string> { "Stage_Final" } }
    };

    // StageDataManager의 orderedStageNames와 동일한 배열 정의
    private readonly string[] orderedStageNames = new string[]
    {
        "Stage_1_1", "Stage_1_2", "Stage_1_3", "Stage_1_4", "Stage_1_Sub1", "Stage_1_Sub2",
        "Stage_2_1", "Stage_2_2", "Stage_2_3", "Stage_2_Sub1", "Stage_2_Sub2", "Stage_3_1",
        "Stage_3_2", "Stage_3_3", "Stage_3_Sub1", "Stage_3_Sub2", "Stage_4_1", "Stage_4_2",
        "Stage_4_3", "Stage_4_4", "Stage_Final"
    };

    // Available 스테이지 목록 반환
    public List<string> GetAvailableStages()
    {
        List<string> availableStages = new List<string>();

        foreach (string stageName in orderedStageNames)
        {
            if (StageDataManager.Instance.GetStageStatus(stageName) == StageStatus.Available)
            {
                availableStages.Add(stageName);
            }
        }

        return availableStages;
    }

    // 각 Available 스테이지가 속한 구간 번호 반환
    public Dictionary<string, int> GetSectionForAvailableStages()
    {
        List<string> availableStages = GetAvailableStages();
        Dictionary<string, int> stageToSection = new Dictionary<string, int>();

        foreach (string stageName in availableStages)
        {
            // stageSections에서 해당 스테이지가 속한 구간 찾기
            foreach (var section in stageSections)
            {
                if (section.Value.Contains(stageName))
                {
                    stageToSection.Add(stageName, section.Key);
                    break;
                }
            }
        }

        return stageToSection;
    }

    // 구간별로 Available 스테이지 그룹화
    public Dictionary<int, List<string>> GroupAvailableStagesBySection()
    {
        List<string> availableStages = GetAvailableStages();
        Dictionary<int, List<string>> groupedBySection = new Dictionary<int, List<string>>();

        // 모든 구간 초기화
        foreach (var section in stageSections)
        {
            groupedBySection[section.Key] = new List<string>();
        }

        // Available 스테이지를 구간별로 분류
        foreach (string stageName in availableStages)
        {
            foreach (var section in stageSections)
            {
                if (section.Value.Contains(stageName))
                {
                    groupedBySection[section.Key].Add(stageName);
                    break;
                }
            }
        }

        return groupedBySection;
    }

    public Dictionary<int, Vector3> GetAveragePositionBySection()
    {
        Dictionary<int, Vector3> averagePositions = new Dictionary<int, Vector3>();

        foreach (var section in stageSections)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;

            foreach (string stageName in section.Value)
            {
                GameObject stageObj = GameObject.Find(stageName);
                if (stageObj != null)
                {
                    sum += stageObj.transform.position;
                    count++;
                }
            }

            if (count > 0)
            {
                averagePositions[section.Key] = sum / count;
            }
            else
            {
                Debug.LogWarning($"구간 {section.Key}에 유효한 Stage 오브젝트가 없습니다.");
            }
        }

        return averagePositions;
    }




    void Start()
    {
        // 기존 디버그 로그
        var stageToSection = GetSectionForAvailableStages();
        foreach (var pair in stageToSection)
        {
            Debug.Log($"현재 Available 상태인 스테이지 {pair.Key}는 구간 {pair.Value}에 속합니다.");
        }

/*        var averagePositions = GetAveragePositionBySection();
        foreach (var kvp in averagePositions)
        {
            Debug.Log($"구간 {kvp.Key}의 평균 위치: {kvp.Value}");
        }*/
        StageZoomIn();

    }


    void StageZoomIn()
    {
        var cameraController = FindAnyObjectByType<CinemachineCameraController>();
        if (cameraController != null)
        {
            var available = GetAvailableStages();
            if (available.Count == 0)
            {
                Debug.Log("모든 스테이지를 클리어했습니다. 줌인 동작을 수행하지 않습니다.");
                return;
            }

            cameraController.MoveToActiveSection(50f);
        }
    }

    bool CheckT1()
    {
        if (!stageSections.ContainsKey(0))
            return false;

        foreach (string stageName in stageSections[0])
        {
            if (StageDataManager.Instance.GetStageStatus(stageName) != StageStatus.Cleared)
            {
                return false;
            }
        }

        return true;
    }

    bool CheckT2()
    {
        if (!stageSections.ContainsKey(2))
            return false;

        foreach (string stageName in stageSections[2])
        {
            if (StageDataManager.Instance.GetStageStatus(stageName) != StageStatus.Cleared)
            {
                return false;
            }
        }

        return true;
    }

    bool CheckT3()
    {
        if (!stageSections.ContainsKey(4))
            return false;

        foreach (string stageName in stageSections[4])
        {
            if (StageDataManager.Instance.GetStageStatus(stageName) != StageStatus.Cleared)
            {
                return false;
            }
        }

        return true;
    }


    bool CheckM1()
    {
        if (!stageSections.ContainsKey(6))
            return false;

        foreach (string stageName in stageSections[6])
        {
            if (StageDataManager.Instance.GetStageStatus(stageName) != StageStatus.Cleared)
            {
                return false;
            }
        }
        
        return true;
    }



    bool CheckM2()
    {
        if (!stageSections.ContainsKey(7))
            return false;

        foreach (string stageName in stageSections[7])
        {
            if (StageDataManager.Instance.GetStageStatus(stageName) != StageStatus.Cleared)
            {
                return false;
            }
        }
        return true;
    }


    public void SetDesignCircleActive()
    {

        GameObject designCircle = FindAnyObjectByType<DesignCircle>().gameObject;


        List<GameObject> circles = new List<GameObject>();
        foreach (Transform child in designCircle.transform)
        {
            circles.Add(child.gameObject);
        }

        circles[0].SetActive(CheckT1());
        circles[1].SetActive(CheckT2());
        circles[2].SetActive(CheckT3());
        circles[3].SetActive(CheckM1());
        circles[4].SetActive(CheckM2());

    }




}