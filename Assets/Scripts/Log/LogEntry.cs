using System;
using System.Collections.Generic;
using Firebase.Firestore;

[Serializable]
[FirestoreData]
public class LogEntry
{
    [FirestoreProperty] public string EventType      { get; set; }
    [FirestoreProperty] public string PlayerGuid     { get; set; }
    [FirestoreProperty] public string SessionId      { get; set; }
    [FirestoreProperty] public long   Timestamp      { get; set; }
    [FirestoreProperty] public int    AttemptIndex   { get; set; }

    // Dictionary 내부 값은 string/int/float/bool/… 등 Firestore 지원 타입만 사용
    [FirestoreProperty] public Dictionary<string, object> Data { get; set; }

    // Firestore는 **매개변수 없는 기본 생성자**가 필요
    public LogEntry() {}

    public LogEntry(EventType type,
        Dictionary<string, object> payload = null,
        int attempt = 0)
    {
        EventType    = type.ToString();
        PlayerGuid   = LocalGuidManager.Guid;
        SessionId    = SessionManager.Id;
        Timestamp    = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        AttemptIndex = attempt;
        Data         = payload ?? new();
    }
}