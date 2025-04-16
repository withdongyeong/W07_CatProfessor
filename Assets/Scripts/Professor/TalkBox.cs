using UnityEngine;
using TMPro;
using System.Collections;

public class TalkBox : MonoBehaviour
{
    public static TalkBox Instance { get; private set; }

    public TextMeshPro textMesh;
    public float displayDuration = 2f;
    public float textSpeed = 0.05f;

    private Coroutine talkCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        textMesh.gameObject.SetActive(false);
    }

    public void Talk(string message)
    {
        if (talkCoroutine != null)
        {
            StopCoroutine(talkCoroutine);
        }

        talkCoroutine = StartCoroutine(DisplayText(message));
    }

    private IEnumerator DisplayText(string message)
    {
        textMesh.gameObject.SetActive(true);
        textMesh.text = "";

        Professor.Instance.SetAnimation(Professor.AnimationType.Talk);
        foreach (char letter in message.ToCharArray())
        {
            textMesh.text += letter;
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        yield return new WaitForSecondsRealtime(displayDuration);
        textMesh.gameObject.SetActive(false);
        Professor.Instance.SetAnimation(Professor.AnimationType.Idle);

    }
}
