#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class BuildSettingsAutoAdder
{
    [MenuItem("Tools/Add All Scenes to Build Settings")]
    public static void AddAllScenes()
    {
        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

        // 🔹 유지해야 할 씬들
        string[] requiredScenes = {
            "Assets/Scenes/Title.unity",
            "Assets/Scenes/StageSelect.unity"
        };

        // 🔹 기본 씬 추가 (존재하는 경우에만 추가)
        foreach (string scenePath in requiredScenes)
        {
            if (File.Exists(scenePath))
            {
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                Debug.Log($"✅ 기본 씬 추가: {scenePath}");
            }
            else
            {
                Debug.LogWarning($"⚠️ 기본 씬이 없음: {scenePath}");
            }
        }

        // 🔹 스테이지 씬 추가 (중복 방지)
        string[] stageScenes = Directory.GetFiles("Assets/Scenes/Stages/", "*.unity", SearchOption.AllDirectories);
        foreach (string scenePath in stageScenes)
        {
            if (!buildScenes.Any(s => s.path == scenePath))
            {
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }
        }

        // 🔹 Build Settings 업데이트
        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log($"✅ {stageScenes.Length}개의 스테이지 씬이 추가됨, 기존 씬 유지 완료!");
    }
}
#endif