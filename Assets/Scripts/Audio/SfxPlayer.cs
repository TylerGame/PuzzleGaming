using UnityEngine;

/// <summary>
/// Plays Sfx sounds. Useful when you need this functionality somewhere you can't directly call
/// the AudioManager, like animation events (this script is used for example for footsteps and the sound of gunshot in animations)
/// </summary>
public class SfxPlayer : MonoBehaviour
{
    /// <summary>
    /// Plays a SFX calling the AudioManager
    /// </summary>
    /// <param name="sfxName">Name of the SFX, as it is in the AudioCompendium</param>
    public void PlaySfx(string sfxName) {
        AudioManager.Instance.PlaySFX(sfxName);
    }
}
