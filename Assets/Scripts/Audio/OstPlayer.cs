using UnityEngine;

/// <summary>
/// Simple script to play an OST
/// </summary>
public class OstPlayer : MonoBehaviour
{
    /// <summary>
    /// Code of the OST to play, as it is in the AudioCompendium
    /// </summary>
    [SerializeField]
    private string ostName;

    private void Start() {
        AudioManager.Instance.PlayMusic(ostName);
    }
}
