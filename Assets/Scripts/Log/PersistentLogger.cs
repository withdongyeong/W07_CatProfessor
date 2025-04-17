using UnityEngine;

public class PersistentLogger
{
    public void LogSessionStart()
    {
        // 세션 시작
        LogManager.Log(EventType.SessionStart);

        // .NET 레벨에서도 잡아두기
        // System.AppDomain.CurrentDomain.ProcessExit += (_,__) => LogSessionEnd();
        Application.quitting += () => LogSessionEnd(); // 2021.2+
    }

    private void LogSessionEnd()
    {
        // 이미 보냈으면 중복 방지
        // if (PlayerPrefs.GetInt("session_ended", 0) == 1) return;

        LogManager.Log(EventType.SessionEnd);
        // PlayerPrefs.SetInt("session_ended", 1);
        // PlayerPrefs.Save(); // 즉시 디스크 기록
    }
}