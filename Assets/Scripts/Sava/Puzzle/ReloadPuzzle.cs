
using System.IO;
using UnityEngine;

/// <summary>
/// Reloads the puzzle. Only used in a dedicated testing scene for the HexPuzzle
/// </summary>
public class ReloadPuzzle : MonoBehaviour
{
    /// <summary>
    /// handler for puzzle xml data 
    /// </summary>
    PuzzleXML puzzleXml;
    /// <summary>
    /// Definition of the puzzle, in a format: <br></br>
    /// 8x8 1,3,tg,tt,up,2 2,2,tg,tt,up,2 3,2,tg,tt,up,2 2,3,tg,tt,up,2 1,1,dn,tt,up,0 1,4,dn,tt,up,0 4,1,dn2,tt,up,0 4,4,dn,tr,up,0
    /// </summary>
    public string definition;

    /// <summary>
    /// Reference to the controller of the puzzle to reload
    /// </summary>
    PuzzleManager controller;

    private void Awake()
    {
        controller = FindObjectOfType<PuzzleManager>();
    }

    /// <summary>
    /// Resets the puzzle
    /// </summary>
    public void ResetPuzzle()
    {
        LoadXML();
        controller.LoadPuzzle(puzzleXml.definition.Trim());
    }

    /// <summary>
    /// Load XML file
    /// </summary>
    void LoadXML()
    {
        puzzleXml = XmlUtils.ImportXml<PuzzleXML>(Path.Combine(Application.absoluteURL, "xmls/puzzle.xml"));
    }
}



