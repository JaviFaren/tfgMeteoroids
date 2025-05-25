using UnityEngine;

public class CharacterSoundFX : MonoBehaviour
{
    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        SetCharacterAudioSourceEnable();
    }

    public void SetCharacterAudioSourceEnable()
    {
        audioSource.mute = !UserSession.SoundFX.Equals("Yes");
    }

    public void PlayFXSound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
