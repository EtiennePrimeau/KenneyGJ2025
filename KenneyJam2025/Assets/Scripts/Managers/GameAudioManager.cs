using System.Collections;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public enum GameState
    {
        Menu, Game
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    
    [Header("Settings")]
    [SerializeField] private float fadeTransitionTime = 1f;
    
    private GameState currentState = GameState.Menu;

    public GameState State => currentState;

    public void SetGameState(GameState newState, bool isLaunching = false)
    {
        if (isLaunching)
        {
            audioSource.loop = true;
            audioSource.clip = menuMusic;
            audioSource.Play();
            return;
        }
        
        if (currentState != newState)
        {
            currentState = newState;
            PlayMusic(currentState);
        }
    }

    private void PlayMusic(GameState state)
    {
        AudioClip clip = null;
        switch (state)
        {
            case GameState.Menu:
                clip = menuMusic;
                break;
            case GameState.Game:
                clip = gameMusic;
                break;
        }
        
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        StartCoroutine(TransitionToMusic(clip));
    }

    private IEnumerator TransitionToMusic(AudioClip newClip)
    {
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeOut());
        }

        audioSource.clip = newClip;
        audioSource.Play();
        
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeTransitionTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTransitionTime);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    private IEnumerator FadeIn()
    {
        float targetVolume = 1f;
        float timer = 0f;
        audioSource.volume = 0f;

        while (timer < fadeTransitionTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeTransitionTime);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            StartCoroutine(FadeOut());
        }
    }

    public void ToggleMute()
    {
        if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
        }
    }
}
