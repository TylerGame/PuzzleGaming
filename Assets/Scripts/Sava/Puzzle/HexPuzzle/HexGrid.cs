using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Representation of a HexGrid for the puzzle
/// </summary>
public class HexGrid
{

    /// <summary>
    /// Size defined as rows by columns
    /// </summary>
    public Vector2Int Size { get; set; }


    /// <summary>
    /// List of HexNodes in the grid
    /// </summary>
    public List<HexNode> Nodes { get; set; }


    /// <summary>
    /// Lookup dictionary to convert the strings in the puzzle text definition to the corresponding HexType
    /// </summary>
    private Dictionary<string, HexType> StringToHexType = new Dictionary<string, HexType>() {
        { "bk", HexType.Blank },
        { "cn", HexType.Circle },
        { "ci", HexType.CircleInverse },
        { "tn", HexType.Triangle },
        { "ti", HexType.TriangleInverse },
        { "pt", HexType.Path },
        { "tg", HexType.Target },
        { "rn", HexType.Rectangle },
        { "ri", HexType.RectangleInverse },
        { "rn2", HexType.Rectangle2 },
        { "ri2", HexType.RectangleInverse2 },
        { "tn2", HexType.Triangle2 },
        { "ti2", HexType.TriangleInverse2 },
        { "dn2", HexType.Directional2 },
        { "di2", HexType.DirectionalInverse2 },
        { "dn", HexType.Directional },
        { "di", HexType.DirectionalInverse },
    };

    /// <summary>
    /// Lookup dictionary to convert the strings in the puzzle text definition to the corresponding HexDirection
    /// </summary>
    private Dictionary<string, HexDirection> StringToHexDirection = new Dictionary<string, HexDirection>() {
        { "tt", HexDirection.Top },
        { "tr", HexDirection.TopRight },
        { "br", HexDirection.BottomRight },
        { "bb", HexDirection.Bottom },
        { "bl", HexDirection.BottomLeft },
        { "tl", HexDirection.TopLeft },
    };

    /// <summary>
    /// Lookup dictionary to convert the strings in the puzzle text definition to the corresponding HexState
    /// </summary>
    private Dictionary<string, HexState> StringToHexState = new Dictionary<string, HexState>() {
        { "up", HexState.Unpowered },
        { "pw", HexState.Powered },
    };

    /// <summary>
    /// Lookup  dictionary to map the counterclockwise next direction of a HexNode
    /// </summary>
    private Dictionary<HexDirection, HexDirection> HexDirectionToNext = new Dictionary<HexDirection, HexDirection>() {
        { HexDirection.TopRight, HexDirection.Top },
        { HexDirection.BottomRight, HexDirection.TopRight },
        { HexDirection.Bottom, HexDirection.BottomRight },
        { HexDirection.BottomLeft, HexDirection.Bottom },
        { HexDirection.TopLeft, HexDirection.BottomLeft },
        { HexDirection.Top, HexDirection.TopLeft },
    };

    public HexGrid(string definition)
    {
        FillGridSimple(definition);
    }

    /// <summary>
    /// Loads the HexGrid from a text string. The format of the string of the type: <br></br>
    /// 8x8 1,3,tg,tt,up,2 2,2,tg,tt,up,2 3,2,tg,tt,up,2 2,3,tg,tt,up,2 1,1,dn,tt,up,0 1,4,dn,tt,up,0 4,1,dn2,tt,up,0 4,4,dn,tr,up,0
    /// </summary>
    /// <param name="definition">Puzzle definition</param>
    public void FillGridSimple(string definition)
    {
        var blocks = definition.Split(' ').ToList();
        Size = new Vector2Int(int.Parse(blocks[1].Split("x")[0]), int.Parse(blocks[1].Split("x")[1]));
        blocks.RemoveRange(0, 2);

        var foundNodes = new List<HexNode>();
        blocks.Select(v => v.Split(',')).ToList()
            .ForEach(n => foundNodes.Add(new HexNode(new Vector2Int(int.Parse(n[0]), int.Parse(n[1])), StringToHexType[n[2]], StringToHexDirection[n[3]], StringToHexState[n[4]] == HexState.Powered ? 1 : 0, int.Parse(n[5]))));

        HexNode nodeToInsert;
        Nodes = new List<HexNode>();
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                nodeToInsert = foundNodes.Where(n => n.Position.x == x && n.Position.y == y).FirstOrDefault();
                if (nodeToInsert is not null)
                {
                    Nodes.Add(nodeToInsert);
                }
                else
                {
                    Nodes.Add(new HexNode(new Vector2Int(x, y), HexType.Path, HexDirection.Top, 0));
                }
            }
        }


    }

    /// <summary>
    /// Returns the HexNode at row x and column y
    /// </summary>
    /// <param name="x">HexNode row</param>
    /// <param name="y">HexNode column</param>
    /// <returns>The HexNode at row x and column y</returns>
    public HexNode GetNode(int x, int y)
    {
        return Nodes[x * Size.x + y];
    }

    /// <summary>
    /// Returns the HexNode at position
    /// </summary>
    /// <param name="position">HexNode position (row, column)</param>
    /// <returns>The HexNode at position (row, column)</returns>
    public HexNode GetNode(Vector2Int position)
    {
        return Nodes[position.x * Size.x + position.y];
    }

    /// <summary>
    /// Rotates the HexNode counterclockwise
    /// </summary>
    /// <param name="hexNode">HexNode to rotate</param>
    /// <returns>The list of neighbor HexNodes that changed because of the rotation</returns>
    public List<HexNode> RotateNode(HexNode hexNode)
    {
        List<HexNode> previousNodes = GetNodesInPowerRange(hexNode);
        hexNode.SetDirection(HexDirectionToNext[hexNode.Direction]);
        List<HexNode> changedNodes = new List<HexNode>();

        if (hexNode.Type == HexType.Path || hexNode.Type == HexType.Target) return null;

        List<HexNode> currentNodes = GetNodesInPowerRange(hexNode);

        foreach (var nb in previousNodes)
        {
            if (nb is null) continue;
            if (nb.Type == HexType.Path)
            {
                nb.PowerCounter -= hexNode.PowerCounter;
                changedNodes.Add(nb);
            }
        }

        foreach (var nb in currentNodes)
        {
            if (nb is null) continue;
            if (nb.Type == HexType.Path)
            {
                nb.PowerCounter += hexNode.PowerCounter;
                if (!changedNodes.Contains(nb))
                    changedNodes.Add(nb);
            }
        }
        return changedNodes;
    }

    /// <summary>
    /// Turns on or off the HexNode
    /// </summary>
    /// <param name="hexNode">HexNode to turn on or off</param>
    /// <returns>The list of neighbor HexNodes that changed because of the switch</returns>
    public List<HexNode> SwitchHexNode(HexNode hexNode)
    {
        List<HexNode> changedNodes = new List<HexNode>();
        if (hexNode.Type == HexType.Path || hexNode.Type == HexType.Target) return null;

        int difference = hexNode.PowerCounter;
        hexNode.SwitchState();
        difference -= hexNode.PowerCounter;

        changedNodes.Add(hexNode);
        List<HexNode> currentNodes = GetNodesInPowerRange(hexNode);

        foreach (var nb in currentNodes)
        {
            if (nb is null) continue;
            if (nb.Type == HexType.Path)
            {
                nb.PowerCounter -= difference;
                changedNodes.Add(nb);
            }
        }
        return changedNodes;
    }

    /// <summary>
    /// Returns the list of HexNodes that might be affected by the HexNode (power-wise)
    /// </summary>
    /// <param name="hexNode">HexNode potentially switched</param>
    /// <returns>The list of HexNodes that might be affected by the HexNode (power-wise)</returns>
    List<HexNode> GetNodesInPowerRange(HexNode hexNode)
    {
        List<HexNode> powerNodes = new List<HexNode>();
        switch (hexNode.Type)
        {
            case HexType.Circle:
            case HexType.CircleInverse:
                powerNodes = GetAllNeighbors(hexNode);
                break;
            case HexType.Triangle:
            case HexType.TriangleInverse:
                if (hexNode.Direction == HexDirection.Top || hexNode.Direction == HexDirection.BottomLeft || hexNode.Direction == HexDirection.BottomRight)
                {
                    powerNodes.Add(GetTopNeighbor(hexNode));
                    powerNodes.Add(GetBottomLeftNeighbor(hexNode));
                    powerNodes.Add(GetBottomRightNeighbor(hexNode));
                }
                else
                {
                    powerNodes.Add(GetBottomNeighbor(hexNode));
                    powerNodes.Add(GetTopLeftNeighbor(hexNode));
                    powerNodes.Add(GetTopRightNeighbor(hexNode));
                }
                break;
            case HexType.Triangle2:
            case HexType.TriangleInverse2:
                if (hexNode.Direction == HexDirection.Top || hexNode.Direction == HexDirection.BottomLeft || hexNode.Direction == HexDirection.BottomRight)
                {
                    powerNodes.Add(GetTopNeighbor(hexNode));
                    powerNodes.Add(GetTopNeighbor(GetTopNeighbor(hexNode)));
                    powerNodes.Add(GetBottomLeftNeighbor(hexNode));
                    powerNodes.Add(GetBottomLeftNeighbor(GetBottomLeftNeighbor(hexNode)));
                    powerNodes.Add(GetBottomRightNeighbor(hexNode));
                    powerNodes.Add(GetBottomRightNeighbor(GetBottomRightNeighbor(hexNode)));
                }
                else
                {
                    powerNodes.Add(GetBottomNeighbor(hexNode));
                    powerNodes.Add(GetBottomNeighbor(GetBottomNeighbor(hexNode)));
                    powerNodes.Add(GetTopLeftNeighbor(hexNode));
                    powerNodes.Add(GetTopLeftNeighbor(GetTopLeftNeighbor(hexNode)));
                    powerNodes.Add(GetTopRightNeighbor(hexNode));
                    powerNodes.Add(GetTopRightNeighbor(GetTopRightNeighbor(hexNode)));
                }
                break;
            case HexType.Rectangle:
            case HexType.RectangleInverse:
                if (hexNode.Direction == HexDirection.Top || hexNode.Direction == HexDirection.Bottom)
                {
                    powerNodes.Add(GetTopNeighbor(hexNode));
                    powerNodes.Add(GetBottomNeighbor(hexNode));
                }
                else if (hexNode.Direction == HexDirection.TopLeft || hexNode.Direction == HexDirection.BottomRight)
                {
                    powerNodes.Add(GetTopLeftNeighbor(hexNode));
                    powerNodes.Add(GetBottomRightNeighbor(hexNode));
                }
                else if (hexNode.Direction == HexDirection.TopRight || hexNode.Direction == HexDirection.BottomLeft)
                {
                    powerNodes.Add(GetTopRightNeighbor(hexNode));
                    powerNodes.Add(GetBottomLeftNeighbor(hexNode));
                }
                break;
            case HexType.Rectangle2:
            case HexType.RectangleInverse2:
                if (hexNode.Direction == HexDirection.Top || hexNode.Direction == HexDirection.Bottom)
                {
                    powerNodes.Add(GetTopNeighbor(hexNode));
                    powerNodes.Add(GetTopNeighbor(GetTopNeighbor(hexNode)));
                    powerNodes.Add(GetBottomNeighbor(hexNode));
                    powerNodes.Add(GetBottomNeighbor(GetBottomNeighbor(hexNode)));
                }
                else if (hexNode.Direction == HexDirection.TopLeft || hexNode.Direction == HexDirection.BottomRight)
                {
                    powerNodes.Add(GetTopLeftNeighbor(hexNode));
                    powerNodes.Add(GetTopLeftNeighbor(GetTopLeftNeighbor(hexNode)));
                    powerNodes.Add(GetBottomRightNeighbor(hexNode));
                    powerNodes.Add(GetBottomRightNeighbor(GetBottomRightNeighbor(hexNode)));
                }
                else if (hexNode.Direction == HexDirection.TopRight || hexNode.Direction == HexDirection.BottomLeft)
                {
                    powerNodes.Add(GetTopRightNeighbor(hexNode));
                    powerNodes.Add(GetTopRightNeighbor(GetTopRightNeighbor(hexNode)));
                    powerNodes.Add(GetBottomLeftNeighbor(hexNode));
                    powerNodes.Add(GetBottomLeftNeighbor(GetBottomLeftNeighbor(hexNode)));
                }
                break;
            case HexType.Directional:
            case HexType.DirectionalInverse:
                if (hexNode.Direction == HexDirection.Top)
                {
                    powerNodes.Add(GetTopNeighbor(hexNode));
                }
                else if (hexNode.Direction == HexDirection.Bottom)
                {
                    powerNodes.Add(GetBottomNeighbor(hexNode));
                }
                else if (hexNode.Direction == HexDirection.TopLeft)
                {
                    powerNodes.Add(GetTopLeftNeighbor(hexNode));
                }
                else if (hexNode.Direction == HexDirection.TopRight)
                {
                    powerNodes.Add(GetTopRightNeighbor(hexNode));
                }
                else if (hexNode.Direction == HexDirection.BottomLeft)
                {
                    powerNodes.Add(GetBottomLeftNeighbor(hexNode));
                }
                else if (hexNode.Direction == HexDirection.BottomRight)
                {
                    powerNodes.Add(GetBottomRightNeighbor(hexNode));
                }
                break;
            case HexType.Directional2:
            case HexType.DirectionalInverse2:
                if (hexNode.Direction == HexDirection.Top)
                {
                    powerNodes.Add(GetTopNeighbor(hexNode));
                    powerNodes.Add(GetTopNeighbor(GetTopNeighbor(hexNode)));
                }
                else if (hexNode.Direction == HexDirection.Bottom)
                {
                    powerNodes.Add(GetBottomNeighbor(hexNode));
                    powerNodes.Add(GetBottomNeighbor(GetBottomNeighbor(hexNode)));
                }
                else if (hexNode.Direction == HexDirection.TopLeft)
                {
                    powerNodes.Add(GetTopLeftNeighbor(hexNode));
                    powerNodes.Add(GetTopLeftNeighbor(GetTopLeftNeighbor(hexNode)));
                }
                else if (hexNode.Direction == HexDirection.TopRight)
                {
                    powerNodes.Add(GetTopRightNeighbor(hexNode));
                    powerNodes.Add(GetTopRightNeighbor(GetTopRightNeighbor(hexNode)));
                }
                else if (hexNode.Direction == HexDirection.BottomLeft)
                {
                    powerNodes.Add(GetBottomLeftNeighbor(hexNode));
                    powerNodes.Add(GetBottomLeftNeighbor(GetBottomLeftNeighbor(hexNode)));
                }
                else if (hexNode.Direction == HexDirection.BottomRight)
                {
                    powerNodes.Add(GetBottomRightNeighbor(hexNode));
                    powerNodes.Add(GetBottomRightNeighbor(GetBottomRightNeighbor(hexNode)));
                }
                break;
        }
        return powerNodes;
    }

    /// <summary>
    /// Returns all 6 neighbors of the HexNode (or less if the HexNode is close to the borders)
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>All 6 neighbors of the HexNode (or less if the HexNode is close to the borders)</returns>
    List<HexNode> GetAllNeighbors(HexNode hexNode)
    {
        return new List<HexNode>() {
            GetTopNeighbor(hexNode),
            GetBottomNeighbor(hexNode),
            GetTopLeftNeighbor(hexNode),
            GetTopRightNeighbor(hexNode),
            GetBottomLeftNeighbor(hexNode),
            GetBottomRightNeighbor(hexNode)
        };
    }

    /// <summary>
    /// Returns the top neighbor of the HexNode (or null if there is none)
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>The top neighbor of the HexNode (or null if there is none)</returns>
    public HexNode GetTopNeighbor(HexNode hexNode)
    {
        int x, y;
        x = hexNode.Position.x - 1;
        y = hexNode.Position.y;
        if (x >= 0)
            return GetNode(x, y);
        return null;
    }

    /// <summary>
    /// Returns the top right neighbor of the HexNode (or null if there is none)
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>The top right neighbor of the HexNode (or null if there is none)</returns>
    public HexNode GetTopRightNeighbor(HexNode hexNode)
    {
        int x, y;
        if (hexNode.Position.y % 2 == 0)
        {
            x = hexNode.Position.x - 1;
            y = hexNode.Position.y + 1;
        }
        else
        {
            x = hexNode.Position.x;
            y = hexNode.Position.y + 1;
        }

        if (x >= 0 && y < Size.y)
            return GetNode(x, y);
        return null;
    }

    /// <summary>
    /// Returns the bottom right neighbor of the HexNode (or null if there is none)
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>The bottom right neighbor of the HexNode (or null if there is none)</returns>
    public HexNode GetBottomRightNeighbor(HexNode hexNode)
    {
        int x, y;
        if (hexNode.Position.y % 2 == 0)
        {
            x = hexNode.Position.x;
            y = hexNode.Position.y + 1;
        }
        else
        {
            x = hexNode.Position.x + 1;
            y = hexNode.Position.y + 1;
        }

        if (x >= 0 && y < Size.y)
            return GetNode(x, y);
        return null;
    }

    /// <summary>
    /// Returns the bottom neighbor of the HexNode (or null if there is none)
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>The bottom neighbor of the HexNode (or null if there is none)</returns>
    public HexNode GetBottomNeighbor(HexNode hexNode)
    {
        int x, y;
        x = hexNode.Position.x + 1;
        y = hexNode.Position.y;
        if (x < Size.x)
            return GetNode(x, y);
        return null;
    }

    /// <summary>
    /// Returns the bottom left neighbor of the HexNode (or null if there is none)
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>The bottom left neighbor of the HexNode (or null if there is none)</returns>
    public HexNode GetBottomLeftNeighbor(HexNode hexNode)
    {
        int x, y;
        if (hexNode.Position.y % 2 == 0)
        {
            x = hexNode.Position.x;
            y = hexNode.Position.y - 1;
        }
        else
        {
            x = hexNode.Position.x + 1;
            y = hexNode.Position.y - 1;
        }

        if (x < Size.x && y >= 0)
            return GetNode(x, y);
        return null;
    }

    /// <summary>
    /// Returns the top left neighbor of the HexNode (or null if there is none)
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>The top left neighbor of the HexNode (or null if there is none)</returns>
    public HexNode GetTopLeftNeighbor(HexNode hexNode)
    {
        int x, y;
        if (hexNode.Position.y % 2 == 0)
        {
            x = hexNode.Position.x - 1;
            y = hexNode.Position.y - 1;
        }
        else
        {
            x = hexNode.Position.x;
            y = hexNode.Position.y - 1;
        }

        if (x >= 0 && y >= 0)
            return GetNode(x, y);
        return null;
    }

    /// <summary>
    /// Returns the total power the HexNode gets from the surrounding nodes
    /// </summary>
    /// <param name="hexNode">Reference HexNode</param>
    /// <returns>The total power the HexNode gets from the surrounding nodes</returns>
    public int GetSurroundingPower(HexNode hexNode)
    {
        Debug.Log("Check surrounding power for target node " + hexNode.Position.x + "," + hexNode.Position.y + ": ");
        Debug.Log("Neighbors: ");
        var neighbors = GetAllNeighbors(hexNode);
        int power = 0;
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i] is null) continue;

            Debug.Log(neighbors[i].Position.x + "," + neighbors[i].Position.y);
            if (neighbors[i].Type == HexType.Path)
            {
                power += neighbors[i].PowerCounter;
            }
        }
        Debug.Log("Total power for target node " + hexNode.Position.x + "," + hexNode.Position.y + ": " + power);
        return power;
    }
}
