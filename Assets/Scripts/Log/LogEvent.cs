using System;
using System.Collections.Generic;
using Firebase.Firestore;

[Serializable]
[FirestoreData]
public class LogEvent
{
    [FirestoreProperty] public string EventType   { get; set; } // "ManaReset", "StageReset", "CircuitClick", …
    [FirestoreProperty] public long   Timestamp   { get; set; }// DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    [FirestoreProperty] public Dictionary<string, object> Data { get; set; }// 예: { {"circuitId","C3"}, {"x",3}, {"y",5} }

    public LogEvent() { }  // 기본 생성자

    public LogEvent(string type, Dictionary<string, object> data = null)
    {
        EventType = type;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Data      = data ?? new Dictionary<string, object>();
    }
}