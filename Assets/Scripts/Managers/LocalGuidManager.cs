using UnityEngine;

public class LocalGuidManager : MonoBehaviour
{
    private const string GuidKey = "LOCAL_GUID";
    private static string _guid;

    public static string Guid
    {
        get
        {
            if (_guid == null) LoadOrCreateGuid();
            return _guid;
        }
    }

    // PlayerPrefs에 저장된 GUID를 불러오거나 새로 생성
    private static void LoadOrCreateGuid()
    {
        if (PlayerPrefs.HasKey(GuidKey))
        {
            _guid = PlayerPrefs.GetString(GuidKey);
        }
        else
        {
            _guid = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(GuidKey, _guid);
            PlayerPrefs.Save();
        }
    }

    /* 진행 데이터 초기화용 */
    public static void ResetGuid()
    {
        PlayerPrefs.DeleteKey(GuidKey);
        _guid = null;
    }
}
