using System.Collections.Generic;
using Firebase.Firestore;

public class StageLogger
{
    private StageLog currentLog;

    public void StartStage(string stageName)
    {
        currentLog = new StageLog(
            stageName,
            LocalGuidManager.Guid,
            SessionManager.Id,
            Professor.Instance.hintType ? "B" : "A"
        );
    }

    public void RecordEvent(string type, Dictionary<string, object> data = null)
    {
        if (currentLog == null) return;
        currentLog.Events.Add(new LogEvent(type, data));
    }

    public async void FlushStage()
    {
        if (currentLog == null || currentLog.Events.Count == 0) return;

        // 1) Firestore에 문서 하나로 저장
        await FirebaseFirestore.DefaultInstance
            .Collection("stageLogs")
            .AddAsync(currentLog);

        // 2) 버퍼 초기화
        currentLog = null;
    }
}