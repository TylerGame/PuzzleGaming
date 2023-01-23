using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexGraphics", menuName = "GlassUtils/Create HexGraphics", order = 50)]
public class HexGraphicsScriptableObject : ScriptableObject
{

    [SerializeField]
    private HexType hexType;
    /// <summary>
    /// Type of the hex
    /// </summary>
    [Tooltip("Type of the hex")]
    public HexType HexType => hexType;

    [SerializeField]
    private Sprite neutralGraphics;
    /// <summary>
    /// Icon of the Hex visible ingame
    /// </summary>
    [Tooltip("Icon of the Hex visible ingame")]
    public Sprite NeutralGraphics => neutralGraphics;

    [SerializeField]
    private Sprite positiveGraphics;
    /// <summary>
    /// Icon of the Hex visible ingame
    /// </summary>
    [Tooltip("Icon of the Hex visible ingame")]
    public Sprite PositiveGraphics => positiveGraphics;

    [SerializeField]
    private Sprite negativeGraphics;
    /// <summary>
    /// Icon of the Hex visible ingame
    /// </summary>
    [Tooltip("Icon of the Hex visible ingame")]
    public Sprite NegativeGraphics => negativeGraphics;

    [SerializeField]
    private Sprite specialGraphics1;
    /// <summary>
    /// Icon of the Hex visible ingame
    /// </summary>
    [Tooltip("Icon of the Hex visible ingame")]
    public Sprite SpecialGraphics1 => specialGraphics1;

    [SerializeField]
    private Sprite specialGraphics2;
    /// <summary>
    /// Icon of the Hex visible ingame
    /// </summary>
    [Tooltip("Icon of the Hex visible ingame")]
    public Sprite SpecialGraphics2 => specialGraphics2;

    [SerializeField]
    private Sprite specialGraphics3;
    /// <summary>
    /// Icon of the Hex visible ingame
    /// </summary>
    [Tooltip("Icon of the Hex visible ingame")]
    public Sprite SpecialGraphics3 => specialGraphics3;

}
