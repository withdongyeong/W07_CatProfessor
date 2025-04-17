using Firebase;
using Firebase.Extensions;
using UnityEngine;

[DefaultExecutionOrder(-100)] // 매우 이른 실행
public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance { get; private set; }
    public static bool Ready { get; private set; }

    private PersistentLogger persistentLogger;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            Ready = task.Result == DependencyStatus.Available;
            Debug.Log(Ready ? "Firebase Ready" : "Firebase NOT Ready");
        });
        
        persistentLogger = new PersistentLogger();
        persistentLogger.LogSessionStart();
    }
}