using System;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseLogService : ILogService
{
    private readonly CollectionReference _root =
        FirebaseFirestore.DefaultInstance.Collection("logs");

    public void Log(LogEntry entry)
    {
        string folder = DateTime.UtcNow.ToString("yyyy-MM-dd");
        _root.Document(folder)
            .Collection("events")
            .AddAsync(entry)
            .ContinueWithOnMainThread(t =>
            {
                if (!t.IsCompletedSuccessfully)
                    UnityEngine.Debug.LogError("Log fail: "+t.Exception);
            });
    }
}