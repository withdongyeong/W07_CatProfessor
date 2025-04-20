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
    [FirestoreProperty] public List<LogEvent> Events { get; set; }
    public StageLog() { }

    public StageLog(string stage, string player, string session)
    {
        StageName  = stage;
        PlayerGuid = player;
        SessionId  = session;
        Events     = new List<LogEvent>();
    }
}