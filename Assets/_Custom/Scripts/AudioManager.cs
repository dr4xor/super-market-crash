using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Global singleton for music and sound effects. Persists across scenes (DontDestroyOnLoad).
/// Supports Unity Audio Mixer for grouping and volume control. Use two music sources to
/// crossfade between Main Menu and Gameplay music in the same scene.
///
/// Audio Mixer setup (optional): Create → Audio → Audio Mixer. Add groups under Master: "Music", "SFX".
/// Expose each group's Volume: in the Audio Mixer window select the group, then right-click the
/// Volume fader (slider) for that group and choose "Expose 'Volume' to script". Open Exposed
/// Parameters (wrench icon / top right), rename to "MusicVolume" and "SFXVolume", then assign
/// the mixer and groups on this component.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum MusicType
    {
        MainMenu,
        Gameplay
    }

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] [Min(0.1f)] private float musicCrossfadeDuration = 2f;

    [Header("Game Sounds")]
    [SerializeField] private AudioClip countdownSound;

    [Header("Audio Mixer (optional)")]
    [Tooltip("Assign a mixer with Music and SFX groups. Expose volume parameters for runtime control.")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";
    [Tooltip("Optional: route music/sfx outputs through these groups.")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Volume (used when no mixer is set)")]
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

    private AudioSource _musicSourceA;
    private AudioSource _musicSourceB;
    private AudioSource _sfxSource;
    private MusicType? _currentMusicType = null;
    private bool _usingSourceA = true;
    private Coroutine _crossfadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    private void SetupAudioSources()
    {
        _musicSourceA = gameObject.AddComponent<AudioSource>();
        _musicSourceB = gameObject.AddComponent<AudioSource>();
        _sfxSource = gameObject.AddComponent<AudioSource>();

        foreach (var source in new[] { _musicSourceA, _musicSourceB })
        {
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
            if (musicMixerGroup != null)
                source.outputAudioMixerGroup = musicMixerGroup;
        }

        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;
        _sfxSource.spatialBlend = 0f;
        if (sfxMixerGroup != null)
            _sfxSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    private void Start()
    {
        if (audioMixer != null)
        {
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
        }
        else
        {
            _musicSourceA.volume = musicVolume;
            _musicSourceB.volume = musicVolume;
            _sfxSource.volume = sfxVolume;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Plays the given music type. If already playing that type, does nothing.
    /// Crossfades when switching between Main Menu and Gameplay in the same scene.
    /// </summary>
    public void PlayMusic(MusicType type)
    {
        if (_currentMusicType == type)
            return;

        AudioClip clip = type == MusicType.MainMenu ? mainMenuMusic : gameplayMusic;
        if (clip == null)
        {
            Debug.LogWarning($"AudioManager: No clip assigned for music type {type}.");
            return;
        }

        _currentMusicType = type;

        if (_crossfadeRoutine != null)
            StopCoroutine(_crossfadeRoutine);

        bool nothingPlaying = !_musicSourceA.isPlaying && !_musicSourceB.isPlaying;
        if (nothingPlaying)
        {
            AudioSource src = _usingSourceA ? _musicSourceA : _musicSourceB;
            src.clip = clip;
            src.loop = true;
            src.volume = audioMixer != null ? 1f : musicVolume;
            src.Play();
            return;
        }

        _crossfadeRoutine = StartCoroutine(CrossfadeMusic(clip));
    }

    /// <summary>
    /// Stops current music.
    /// </summary>
    public void StopMusic()
    {
        if (_crossfadeRoutine != null)
        {
            StopCoroutine(_crossfadeRoutine);
            _crossfadeRoutine = null;
        }

        _musicSourceA.Stop();
        _musicSourceB.Stop();
        _currentMusicType = null;
    }

    /// <summary>
    /// Fades out the current music over the specified duration.
    /// </summary>
    public void FadeOutMusic(float duration = -1f)
    {
        if (duration < 0f)
            duration = musicCrossfadeDuration;

        if (_crossfadeRoutine != null)
        {
            StopCoroutine(_crossfadeRoutine);
            _crossfadeRoutine = null;
        }

        _crossfadeRoutine = StartCoroutine(FadeOutMusicCoroutine(duration));
    }

    private IEnumerator FadeOutMusicCoroutine(float duration)
    {
        AudioSource activeSource = _musicSourceA.isPlaying ? _musicSourceA : (_musicSourceB.isPlaying ? _musicSourceB : null);
        if (activeSource == null)
        {
            _crossfadeRoutine = null;
            yield break;
        }

        float startVolume = activeSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            activeSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = audioMixer != null ? 1f : musicVolume;

        // Update _usingSourceA to point to the source we just stopped, so the next PlayMusic uses it
        _usingSourceA = (activeSource == _musicSourceA);

        _currentMusicType = null;
        _crossfadeRoutine = null;
    }

    /// <summary>
    /// Plays a one-shot sound effect (global, 2D). Use for UI and global SFX.
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        if (audioMixer == null)
            _sfxSource.volume = sfxVolume;

        _sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Plays the countdown sound effect used before game start.
    /// </summary>
    public void PlayCountdownSound()
    {
        if (countdownSound == null)
        {
            Debug.LogWarning("AudioManager: No countdown sound assigned.");
            return;
        }

        PlaySFX(countdownSound);
    }

    /// <summary>
    /// Sets music volume. Linear 0–1. Only has effect when using an Audio Mixer with exposed parameter.
    /// </summary>
    public void SetMusicVolume(float linearVolume)
    {
        musicVolume = Mathf.Clamp01(linearVolume);
        if (audioMixer != null && audioMixer.GetFloat(musicVolumeParameter, out _))
            audioMixer.SetFloat(musicVolumeParameter, LinearToDb(musicVolume));
        else if (audioMixer == null)
        {
            _musicSourceA.volume = musicVolume;
            _musicSourceB.volume = musicVolume;
        }
    }

    /// <summary>
    /// Sets SFX volume. Linear 0–1. Only has effect when using an Audio Mixer with exposed parameter.
    /// </summary>
    public void SetSFXVolume(float linearVolume)
    {
        sfxVolume = Mathf.Clamp01(linearVolume);
        if (audioMixer != null && audioMixer.GetFloat(sfxVolumeParameter, out _))
            audioMixer.SetFloat(sfxVolumeParameter, LinearToDb(sfxVolume));
        else if (audioMixer == null)
            _sfxSource.volume = sfxVolume;
    }

    /// <summary>
    /// Converts linear volume (0–1) to dB for the Audio Mixer. Use 0.0001 for near-silent to avoid log(0).
    /// </summary>
    public static float LinearToDb(float linear)
    {
        return linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
    }

    private IEnumerator CrossfadeMusic(AudioClip nextClip)
    {
        AudioSource fadeOut = _usingSourceA ? _musicSourceA : _musicSourceB;
        AudioSource fadeIn = _usingSourceA ? _musicSourceB : _musicSourceA;
        _usingSourceA = !_usingSourceA;

        // When using mixer, source volumes stay at 1 and mixer controls level; otherwise we use musicVolume.
        float startVolumeOut = audioMixer != null ? 1f : fadeOut.volume;
        float targetVolumeIn = audioMixer != null ? 1f : musicVolume;

        fadeIn.volume = 0f;
        fadeIn.clip = nextClip;
        fadeIn.loop = true;
        fadeIn.Play();

        float elapsed = 0f;
        float duration = musicCrossfadeDuration;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            fadeOut.volume = Mathf.Lerp(startVolumeOut, 0f, t);
            fadeIn.volume = Mathf.Lerp(0f, targetVolumeIn, t);
            yield return null;
        }

        fadeOut.Stop();
        fadeIn.volume = targetVolumeIn;
        _crossfadeRoutine = null;
    }
}
