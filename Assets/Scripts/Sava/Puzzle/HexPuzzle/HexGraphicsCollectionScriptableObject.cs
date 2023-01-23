using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject to hold all graphics information for the hex puzzle
/// </summary>
[CreateAssetMenu(fileName = "HexGraphicsCollection", menuName = "GlassUtils/Create HexGraphicsCollection", order = 51)]
public class HexGraphicsCollectionScriptableObject : ScriptableObject
{
    [SerializeField]
    private List<HexGraphicsScriptableObject> hexGraphicsList;
    /// <summary>
    /// Collection of all sprites for the hex puzzle
    /// </summary>
    [Tooltip("Collection of all sprites for the hex puzzle")]
    public List<HexGraphicsScriptableObject> HexGraphicsList => hexGraphicsList;
}
