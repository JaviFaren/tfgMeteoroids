using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private AudioSource audioSource;

    public AudioClip ButtonClick;

    [Header("Menu")]
    [SerializeField] private List<AudioClip> menuMusic = new();

    [Header("Juego")]
    [SerializeField] private List<AudioClip> gameMusic = new();

    private Coroutine musicLoopCoroutine;

    private void Awake()
    {
        //  Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetMusicAudioSourceEnable();
    }

    public void SetMusicAudioSourceEnable()
    {
        audioSource.mute = !UserSession.SoundMusic.Equals("Yes");
    }

    public void PlayFXSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    #region MENU
    public void StartMenuMusicLoop()
    {
        musicLoopCoroutine ??= StartCoroutine(PlayMenuMusicLoop());
    }

    public void StopMenuMusicLoop()
    {
        if (musicLoopCoroutine != null)
        {
            StopCoroutine(musicLoopCoroutine);
            musicLoopCoroutine = null;
            audioSource.Stop();
        }
    }

    public IEnumerator PlayMenuMusicLoop()
    {
        while (true)
        {
            audioSource.clip = GetRandomMenuMusic();
            audioSource.Play();

            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    public AudioClip GetRandomMenuMusic() => menuMusic[Random.Range(0, menuMusic.Count)];
    #endregion

    #region GAME
    public void StartGameMusicLoop()
    {
        musicLoopCoroutine ??= StartCoroutine(PlayGameMusicLoop());
    }

    public void StopGameMusicLoop()
    {
        if (musicLoopCoroutine != null)
        {
            StopCoroutine(musicLoopCoroutine);
            musicLoopCoroutine = null;
            audioSource.Stop();
        }
    }

    public IEnumerator PlayGameMusicLoop()
    {
        while (true)
        {
            audioSource.clip = GetRandomGameMusic();
            audioSource.Play();

            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    public AudioClip GetRandomGameMusic() => gameMusic[Random.Range(0, gameMusic.Count)];
    #endregion
}
