using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexagonalPuzzle : PuzzleBase
{

    /// <summary>
    /// Collection of Sprites used in the puzzle
    /// </summary>
    [SerializeField]
    private HexGraphicsCollectionScriptableObject HexSprites;

    /// <summary>
    /// Lookup dictionary of the Sprites used by a hex node by its type
    /// </summary>
    public Dictionary<HexType, HexGraphicsScriptableObject> HexSpritesDictionary { get; private set; }

    /// <summary>
    /// Reference to the prefab of the hexagonal node
    /// </summary>
    public GameObject hexNodePrefab;


    /// <summary>
    /// First grid representing the puzzle. Two grids are used to simulate a single hexagonal grid
    /// </summary>
    private RectTransform grid1;

    /// <summary>
    /// Second grid representing the puzzle. Two grids are used to simulate a single hexagonal grid
    /// </summary>
    private RectTransform grid2;

    /// <summary>
    /// Reference to the HexGrid containing all the data of the puzzle (nodes and their state)
    /// </summary>
    private HexGrid Grid;

    /// <summary>
    /// List of all UI representations of the nodes
    /// </summary>
    private List<HexNodeUI> GridUI;


    /// <summary>
    /// List of all UI representations of the target nodes
    /// </summary>
    private List<HexNodeUI> TargetNodes;

    /// <summary>
    /// Action to perform when the puzzle is completed
    /// </summary>
    private Action onCompletePuzzle;

    /// <summary>
    /// Has the puzzle been completed?
    /// </summary>
    public bool PuzzleCompleted { get; private set; }

    /// <summary>
    /// Lookup dictionary HexDirection->Degrees
    /// </summary>
    private Dictionary<HexDirection, float> HexDirectionInDegrees = new Dictionary<HexDirection, float>() {
        { HexDirection.Top, 0f },
        { HexDirection.TopLeft, 60f },
        { HexDirection.BottomLeft, 120f },
        { HexDirection.Bottom, 180f },
        { HexDirection.BottomRight, 240f },
        { HexDirection.TopRight, 300f },
    };

    /// <summary>
    /// Create Hexagonal Grid
    /// </summary>
    public override void CreateGrid(string definition, Action onComplete = null)
    {
        if (GridUI is not null)
        {
            for (int i = 0; i < GridUI.Count; i++)
                Destroy(GridUI[i].gameObject);

            GridUI.Clear();
            TargetNodes.Clear();
        }

        PuzzleCompleted = false;
        onCompletePuzzle = onComplete;

        base.CreateGrid(definition);
        Grid = new HexGrid(definition);
    }

    public override void GenerateNodesUI()
    {
        HexSpritesDictionary = HexSprites.HexGraphicsList.ToDictionary(x => x.HexType, x => x);
        grid1 = transform.Find("Grid1").GetComponent<RectTransform>();
        grid2 = transform.Find("Grid2").GetComponent<RectTransform>();
        bool flip = true;
        HexNode hexNode;
        GridUI = new List<HexNodeUI>();
        TargetNodes = new List<HexNodeUI>();
        for (int x = 0; x < Grid.Size.x; x++)
        {
            for (int y = 0; y < Grid.Size.y; y++)
            {
                hexNode = Grid.GetNode(x, y);
                hexNode.SetPosition(new Vector2Int(x, y));

                GameObject node = GameObject.Instantiate(hexNodePrefab);
                node.name = x + "," + y;
                RectTransform rt = node.GetComponent<RectTransform>();
                rt.localScale = Vector3.one;
                rt.localRotation = Quaternion.Euler(0f, 0f, HexDirectionInDegrees[hexNode.Direction]);
                Image im = rt.GetComponent<Image>();
                im.sprite = HexSpritesDictionary[hexNode.Type].NeutralGraphics;
                im.color = Color.white;
                im.preserveAspect = true;
                HexNodeUI hexNodeUI = rt.GetComponent<HexNodeUI>();
                hexNodeUI.Initialize(this, hexNode, HexSpritesDictionary[hexNode.Type]);
                GridUI.Add(hexNodeUI);

                if (hexNode.Type == HexType.Target)
                    TargetNodes.Add(hexNodeUI);

                if (flip)
                    rt.SetParent(grid1, false);
                else
                    rt.SetParent(grid2, false);

                flip = !flip;
            }
        }
    }

    /// <summary>
    /// Rotates the node counterclockwise
    /// </summary>
    /// <param name="hexNode">HexNode to rotate</param>
    public void RotateHex(HexNode hexNode)
    {
        AudioManager.Instance.PlaySFX("feedbackChoice");
        var hexNodesToSwitch = Grid.RotateNode(hexNode);
        if (hexNodesToSwitch is null) return;
        foreach (var hn in hexNodesToSwitch)
        {
            GridUI[hn.Position.x * Grid.Size.x + hn.Position.y].SwitchPower();
        }

        CheckCompletion();
    }

    /// <summary>
    /// Turns on/off the power for a node
    /// </summary>
    /// <param name="hexNode">The HexNode to turn on/off</param>
    public void SwitchHex(HexNode hexNode)
    {
        var hexNodesToSwitch = Grid.SwitchHexNode(hexNode);
        if (hexNodesToSwitch is null) return;
        AudioManager.Instance.PlaySFX("continue1");
        foreach (var hn in hexNodesToSwitch)
        {
            GridUI[hn.Position.x * Grid.Size.x + hn.Position.y].SwitchPower();
        }

        CheckCompletion();
    }


    /// <summary>
    /// Checks if all target nodes have the required amount of power
    /// </summary>
    public override void CheckCompletion()
    {
        bool completedPuzzle = true;
        bool partialComplete;
        foreach (var hn in TargetNodes)
        {
            partialComplete = hn.UpdateTargetPower(Grid.GetSurroundingPower(hn.hexNode));
            completedPuzzle = completedPuzzle && partialComplete;
        }

        if (completedPuzzle)
        {
            Debug.Log("COMPLETED PUZZLE!!!!");
            PuzzleCompleted = true;
            AudioManager.Instance.PlaySFX("clockworkMechanism");
            GetComponent<UIAnimation>().TriggerAnimation(1);
            if (onCompletePuzzle is not null)
                onCompletePuzzle.Invoke();
        }
    }


}
