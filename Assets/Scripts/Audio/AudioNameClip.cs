using System;
using UnityEngine;

/// <summary>
/// Simple Dictionary-Like data structure to hold audio tracks
/// </summary>
[Serializable]
public class AudioNameClip {

    [SerializeField]
    private string name;
    /// <summary>
    /// Name of the track
    /// </summary>
    public string Name => name;

    [SerializeField]
    private AudioClip clip;
    /// <summary>
    /// Audio clip
    /// </summary>
    public AudioClip Clip => clip;
}
