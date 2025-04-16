using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip bgmClip;        // 기본 배경음악
    public AudioClip clearMusicClip; // 게임 클리어 음악
    public AudioClip clickSoundClip; // 클릭 효과음

    private AudioSource bgmSource;   // 배경음악 오디오 소스
    private AudioSource sfxSource;   // 효과음 오디오 소스

    void Awake()
    {
        // 싱글톤 유지 - 기존 인스턴스가 있으면 새로 생성된 것은 삭제
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 변경 시에도 유지

        // 오디오 소스 초기화
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        sfxSource.loop = false;

        PlayBGM();
    }

    void PlayBGM()
    {
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }

    public void PlayClearMusic()
    {
        if (clearMusicClip != null)
        {
            sfxSource.PlayOneShot(clearMusicClip); // 배경음악 유지하면서 클리어 음악 1회 재생
        }
    }

    public void PlayClickSound()
    {
        if (clickSoundClip != null)
        {
            sfxSource.PlayOneShot(clickSoundClip); // 배경음악 유지하면서 클릭 효과음 1회 재생
        }
    }
}
