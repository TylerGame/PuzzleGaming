using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ScriptableObject to hold audio files
/// </summary>
[CreateAssetMenu(fileName = "AudioCompendium", menuName = "GlassUtils/Create AudioCompendium", order = 15)]
public class AudioCompendiumScriptableObject : ScriptableObject {

    /// <summary>
    /// List of audio tracks. It should not be like this, it should be a readonly property or similar
    /// </summary>
    [SerializeField]
    public List<AudioNameClip> sounds = new List<AudioNameClip>();
    
    /// <summary>
    /// Returns an audio track from the compendium
    /// </summary>
    /// <param name="name">Name of the track</param>
    /// <returns>The AudioClip for the requested track</returns>
    public AudioClip GetAudioClip(string name) {
        return sounds.Where(x => x.Name == name).FirstOrDefault()?.Clip;
    }
}
