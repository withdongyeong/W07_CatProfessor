using System;
using System.Collections.Generic;
using Firebase.Firestore;

[Serializable]
[FirestoreData]
public class StageLog
{
    [FirestoreProperty] public string StageName   { get; set; }
    [FirestoreProperty] public string PlayerGuid  { get; set; }
    [FirestoreProperty] public string SessionId   { get; set; }
    [FirestoreProperty] public string HintType    { get; set; }     // "A" or "B"
    [FirestoreProperty] public List<LogEvent> Events { get; set; }
    public StageLog() { }

    public StageLog(string stage, string player, string session, string hintType)
    {
        StageName  = stage;
        PlayerGuid = player;
        SessionId  = session;
        HintType   = hintType;
        Events     = new List<LogEvent>();
    }
}