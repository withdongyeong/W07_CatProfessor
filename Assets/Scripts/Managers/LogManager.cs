using System.Collections.Generic;
using UnityEngine;

public class LogManager
{
    private static readonly ILogService _svc = new FirebaseLogService();

    public static void Log(EventType type, Dictionary<string, object> payload = null, int attempt = 0)
    {
        var hintType = Professor.Instance.hintType ? "B" : "A";
        _svc.Log(new LogEntry(type, hintType , payload, attempt));
    }
}
