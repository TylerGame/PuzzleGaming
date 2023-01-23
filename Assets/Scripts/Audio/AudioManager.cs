using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Handles all playback of sound effects and soundtracks/ambience in a centralized way
/// </summary>
public class AudioManager : Singleton<AudioManager> {

    /// <summary>
    /// Reference to the audio mixer
    /// </summary>
    [SerializeField]
    private AudioMixer gameAudioMixer;

    /// <summary>
    /// Audio source dedicated to ambience/environmental soundtracks| CURRENTLY NOT USED, MUST BE REMOVED
    /// AND ALSO THE DICTIONARIES USING IT HAVE TO BE REVIEWED
    /// </summary>
    [SerializeField]
    private AudioSource ambienceAudioSource;

    /// <summary>
    /// Audio sources dedicated to ambience/environmental soundtracks
    /// </summary>
    private List<AudioSource> ambienceAudioSources;

    /// <summary>
    /// Audio source dedicated to the main soundtracks
    /// </summary>
    [SerializeField]
    private AudioSource musicAudioSource;

    /// <summary>
    /// Audio source dedicated to sound effects
    /// </summary>
    [SerializeField]
    private AudioSource sfxAudioSource;

    /// <summary>
    /// Audio mixer group  dedicated to ambience/environmental soundtracks
    /// </summary>
    [SerializeField]
    private AudioMixerGroup ambienceAudioMixerGroup;

    /// <summary>
    /// Audio nixer group dedicated to the main soundtracks
    /// </summary>
    [SerializeField]
    private AudioMixerGroup musicAudioMixerGroup;

    /// <summary>
    /// Audio mixer group dedicated to sound effects
    /// </summary>
    [SerializeField]
    private AudioMixerGroup sfxAudioMixerGroup;

    /// <summary>
    /// List of all sound assets for ambience
    /// </summary>
    [SerializeField]
    private AudioCompendiumScriptableObject ambienceCompendium;

    /// <summary>
    /// List of all sound assets for OST
    /// </summary>
    [SerializeField]
    private AudioCompendiumScriptableObject musicCompendium;

    /// <summary>
    /// List of all sound assets for SFX
    /// </summary>
    [SerializeField]
    private AudioCompendiumScriptableObject sfxCompendium;

    /// <summary>
    /// Sounds divided by category
    /// </summary>
    Dictionary<AudioCategory, AudioCompendiumScriptableObject> AudioCategoryToCompendium;

    /// <summary>
    /// The name of the volume parameter for each sound category
    /// </summary>
    Dictionary<AudioCategory, string> AudioCategoryToVolumeParameter;

    /// <summary>
    /// Temporary variable to hold the played SFX
    /// </summary>
    AudioNameClip sfxClip;

    /// <summary>
    /// Since SFXs are played frequently, a Dictionary is allocated to allow faster access, 
    /// instead to use the AudioCompendium.GetAudioClip method.
    /// This might be extended also to ambience and music sounds.
    /// </summary>
    Dictionary<string, AudioNameClip> sfxDictionary;

    private void Awake() {
        AudioCategoryToCompendium = new Dictionary<AudioCategory, AudioCompendiumScriptableObject>() {
            { AudioCategory.Ambience, ambienceCompendium },
            { AudioCategory.Music, musicCompendium },
            { AudioCategory.SFX, sfxCompendium }
        };

        AudioCategoryToVolumeParameter = new Dictionary<AudioCategory, string>() {
            { AudioCategory.Ambience, "AmbienceVolume" },
            { AudioCategory.Music, "MusicVolume" },
            { AudioCategory.SFX, "SFXVolume" }
        };

        // We can have multiple ambience soundtracks playing simultaneously (dogs barking, rain, lamps gas),
        // so we need multiple sources to handle them
        ambienceAudioSources = new List<AudioSource>();
        for (int i = 0; i < 3; i++) {
            var audio = gameObject.AddComponent<AudioSource>();
            audio.outputAudioMixerGroup = ambienceAudioMixerGroup;
            audio.loop = true;
            audio.playOnAwake = false;
            ambienceAudioSources.Add(audio);
        }

        // SFXs get their dedicated dictionary
        sfxDictionary = sfxCompendium.sounds.ToDictionary(x => x.Name, x => x);
    }

    /// <summary>
    /// Plays a SFX
    /// </summary>
    /// <param name="sfx">The audio clip of the SFX</param>
    public void PlaySFX(AudioClip sfx) {
        if (sfxAudioSource.isPlaying && sfxAudioSource.clip?.name == sfx.name)
            return;

        sfxAudioSource.PlayOneShot(sfx);
    }

    /// <summary>
    /// Plays a SFX. If pitchPool is > 0 it tries to play also variations of that sound
    /// </summary>
    /// <param name="name">The name of the SFX, as set in the AudioCompendium ScriptableObject</param>
    /// <param name="pitchPool">Number of variations this sound has</param>
    public void PlaySFX(string name, int pitchPool = 0) {
        if (pitchPool != 0)
            sfxDictionary.TryGetValue(name + UnityEngine.Random.Range(1, pitchPool+1).ToString(), out sfxClip);
        else
            sfxDictionary.TryGetValue(name, out sfxClip);
        if (sfxClip != null) {
            if (sfxAudioSource.isPlaying && sfxAudioSource.clip?.name == sfxClip.Name)
                return;
            sfxAudioSource.PlayOneShot(sfxClip.Clip);
        }
    }

    /// <summary>
    /// Plays the "pick" sound for an item
    /// </summary>
    /// <param name="name">Name of the item</param>
    public void PlayPick(string name) {
        name += "Pick";
        PlaySFX(name);
    }

    /// <summary>
    /// Plays the "drop" sound for an item
    /// </summary>
    /// <param name="name">Name of the item</param>
    public void PlayDrop(string name) {
        name += "Drop";
        PlaySFX(name);
    }

    /// <summary>
    /// Plays a OST
    /// </summary>
    /// <param name="music">The audio clip of the OST</param>
    public void PlayMusic(AudioClip music) {
        musicAudioSource.clip = music;
        musicAudioSource.Play();
    }

    /// <summary>
    /// Plays a OST
    /// </summary>
    /// <param name="name">The name of the OST, as set in the AudioCompendium ScriptableObject</param>
    public void PlayMusic(string name) {
        AudioClip clip = musicCompendium.GetAudioClip(name);
        if (clip != null) {
            musicAudioSource.clip = clip;
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }

    /// <summary>
    /// Plays or Stops a OST, with a fade
    /// </summary>
    /// <param name="name">C</param>
    /// <param name="enable">If true, the music is played, else it is stopped</param>
    /// <param name="fade">Fade value in seconds</param>
    public void PlayMusic(string name, bool enable, float fade) {
        // If there is a music playing already, need to fadeout that and then fadein the new one
        // else just fadein this one
        if (enable) {
            if (musicAudioSource.isPlaying) {
                StartCoroutine(FadeOutFadeInMusic(name, fade));
            }
            else {
                FadeInMusic(name, fade);
            }
        }
        else {
            FadeOutMusic(fade);
        }
    }

    /// <summary>
    /// Plays or Stops an Ambience track
    /// </summary>
    /// <param name="name">The name of the ambience track, as set in the AudioCompendium ScriptableObject</param>
    /// <param name="enable">If true, the ambience track is played, else it is stopped</param>
    /// <param name="fade">Currently not used</param>
    public void PlayAmbience(string name, bool enable, float fade) {
        var clip = ambienceCompendium.GetAudioClip(name);
        if (clip == null) return;

        for (int i = 0; i < ambienceAudioSources.Count; i++) {
            if (enable) {
                if (!ambienceAudioSources[i].isPlaying) {
                    ambienceAudioSources[i].clip = clip;
                    ambienceAudioSources[i].Play();
                    return;
                }
            }
            else {
                if (ambienceAudioSources[i].clip?.name == clip.name) {
                    ambienceAudioSources[i].Stop();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Plays a track. It's supposed to work with all 3 types but actually right now it works only for OST
    /// </summary>
    /// <param name="audioCategory">The type of track. Right now it works only for Music</param>
    /// <param name="name">The name of the audio track, as set in the AudioCompendium ScriptableObject</param>
    private void Play(AudioCategory audioCategory, string name) {
        AudioClip clip = AudioCategoryToCompendium[audioCategory].GetAudioClip(name);
        AudioSource audioSource = musicAudioSource;
        if (clip != null) {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Fades out the music currently playing
    /// </summary>
    /// <param name="duration">Duration in seconds of the fadeout</param>
    public void FadeOutMusic(float duration) {
        StartCoroutine(FadeOut(AudioCategory.Music, duration));
    }

    /// <summary>
    /// Fades in a music
    /// </summary>
    /// <param name="musicName">The name of the audio track, as set in the AudioCompendium ScriptableObject</param>
    /// <param name="duration">Duration in seconds of the fadein</param>
    public void FadeInMusic(string musicName, float duration) {
        StartCoroutine(FadeIn(musicName, AudioCategory.Music, duration));
    }

    /// <summary>
    /// Fades out the currently playing music, and then fades in a new music
    /// </summary>
    /// <param name="musicName">The name of the audio track, as set in the AudioCompendium ScriptableObject</param>
    /// <param name="duration">Duration in seconds of the fadein/fadeout</param>
    /// <returns></returns>
    private IEnumerator FadeOutFadeInMusic(string musicName, float duration) {
        FadeOutMusic(duration);
        yield return new WaitForSeconds(duration+0.2f);
        FadeInMusic(musicName, duration);
    }

    /// <summary>
    /// Fades in a track (Music)
    /// </summary>
    /// <param name="audioName">The name of the audio track, as set in the AudioCompendium ScriptableObject</param>
    /// <param name="audioCategory">The category of the audio track (right now it works only for Music)</param>
    /// <param name="duration">Duration in seconds of the fadein</param>
    /// <returns></returns>
    public IEnumerator FadeIn(string audioName, AudioCategory audioCategory, float duration) {
        float currentTime = 0;
        float currentVol = 0.0001f;
        float targetVol = 1f;
        string exposedParam = AudioCategoryToVolumeParameter[audioCategory];
        float newVol;

        Play(audioCategory, audioName);

        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            newVol = Mathf.Lerp(currentVol, targetVol, currentTime / duration);
            // The volume has to set in logarithmic scale (decibels/dB)
            gameAudioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20f);
            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// Fades out a track (Music)
    /// </summary>
    /// <param name="audioCategory">The category of the audio track (right now it works only for Music)</param>
    /// <param name="duration">Duration in seconds of the fadeout</param>
    /// <returns></returns>
    public IEnumerator FadeOut(AudioCategory audioCategory, float duration) {
        float currentTime = 0;
        float currentVol = 1f;
        float targetVol = 0.0001f;
        string exposedParam = AudioCategoryToVolumeParameter[audioCategory];
        float newVol;

        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            newVol = Mathf.Lerp(currentVol, targetVol, currentTime / duration);
            gameAudioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20f);
            yield return null;
        }
        musicAudioSource.Stop();
        yield break;
    }
}

/// <summary>
/// Audio track category (Ambience, Music, SFX)
/// </summary>
public enum AudioCategory {
    Ambience,
    Music,
    SFX
}
