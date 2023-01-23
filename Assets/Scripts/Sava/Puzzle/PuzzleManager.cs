using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : Singleton<PuzzleManager>
{

    PuzzleBase puzzle;
    PuzzleBase[] puzzles;

    /// <summary>
    /// Lookup dictionary to convert the strings in the puzzle text definition to the corresponding PuzzleType
    /// </summary>
    private Dictionary<string, PuzzleType> StringToPuzzleType = new Dictionary<string, PuzzleType>() {
        { "hex", PuzzleType.Hexagonal },
        { "sqr", PuzzleType.Hexagonal }
    };

    void Start()
    {
        puzzles = FindObjectsOfType<PuzzleBase>();
        foreach (PuzzleBase p in puzzles)
            p.gameObject.SetActive(false);
    }



    /// <summary>
    /// Initializes the puzzle, both frontend and backend
    /// </summary>
    /// <param name="definition">String containing the definition of the puzzle</param>
    public void LoadPuzzle(string definition)
    {
        puzzle = GetPuzzle(definition);
        StartCoroutine(iLoadPuzzle(definition));
    }

    IEnumerator iLoadPuzzle(string definition)
    {
        yield return null;
        puzzle.CreateGrid(definition);
        puzzle.GenerateNodesUI();
    }
    PuzzleBase GetPuzzle(string definition)
    {
        var blocks = definition.Split(' ').ToList();
        PuzzleBase ret = null;
        switch (StringToPuzzleType[blocks[0]])
        {
            case PuzzleType.Hexagonal:
                puzzles.Where(p => p is HexagonalPuzzle).FirstOrDefault().gameObject.SetActive(true);
                ret = HexagonalPuzzle.Instance;
                break;
            case PuzzleType.Square:
                puzzles.Where(p => p is SquarePuzzle).FirstOrDefault().gameObject.SetActive(true);
                ret = SquarePuzzle.Instance;
                break;
        }
        return ret;
    }
}


public enum PuzzleType
{
    Hexagonal,
    Square
}