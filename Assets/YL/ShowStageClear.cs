using UnityEngine;

public class ShowStageClear : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.parent == transform) // 직속 자식인지 확인
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ShowClear()
    {
            foreach (Transform child in transform)
            {
                if (child.parent == transform) // 직속 자식인지 확인
                {
                    child.gameObject.SetActive(true);
                }
            }
    }

    public void HideClear()
    {
        foreach (Transform child in transform)
        {
            if (child.parent == transform) // 직속 자식인지 확인
            {
                child.gameObject.SetActive(false);
            }
        }
    }


}
