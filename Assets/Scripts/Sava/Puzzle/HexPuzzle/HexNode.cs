using UnityEngine;

/// <summary>
/// Representation of a HexNode for the puzzle
/// </summary>
public class HexNode
{

    /// <summary>
    /// Direction the HexNode is currently in
    /// </summary>
    public HexDirection Direction { get; private set; }

    /// <summary>
    /// Type of the HexNode (basically how it powers nearby nodes)
    /// </summary>
    public HexType Type { get; private set; }

    /// <summary>
    /// Position of the HexNode on the HexGrid
    /// </summary>
    public Vector2Int Position { get; private set; }

    /// <summary>
    /// Power stored by the HexNode
    /// </summary>
    public int PowerCounter = 0;

    /// <summary>
    /// Target Power the HexNode needs to reach. Only for the Target HexType
    /// </summary>
    public int TargetPower { get; private set; }

    public HexNode(HexType type, HexDirection direction, int power)
    {
        Type = type;
        Direction = direction;
        PowerCounter = power;
    }

    public HexNode(Vector2Int position, HexType type, HexDirection direction, int power, int targetPower = 0)
    {
        Position = position;
        Type = type;
        Direction = direction;
        PowerCounter = power;
        TargetPower = targetPower;
    }

    /// <summary>
    /// Sets the direction of the HexNode
    /// </summary>
    /// <param name="direction">New direction of the HexNode</param>
    public void SetDirection(HexDirection direction)
    {
        Direction = direction;
    }

    /// <summary>
    /// Sets the position of the HexNode
    /// </summary>
    /// <param name="position">New position of the HexNode</param>
    public void SetPosition(Vector2Int position)
    {
        Position = position;
    }

    /// <summary>
    /// Switches the power state of the HexNode
    /// </summary>
    public void SwitchState()
    {
        if (Type == HexType.Rectangle || Type == HexType.Triangle ||
            Type == HexType.Circle || Type == HexType.Rectangle2 ||
            Type == HexType.Directional ||
            Type == HexType.Triangle2 || Type == HexType.Directional2)
            PowerCounter = PowerCounter > 0 ? 0 : 1;
        else if (Type == HexType.RectangleInverse || Type == HexType.TriangleInverse ||
            Type == HexType.CircleInverse || Type == HexType.RectangleInverse2 ||
            Type == HexType.DirectionalInverse ||
            Type == HexType.TriangleInverse2 || Type == HexType.DirectionalInverse2)
            PowerCounter = PowerCounter < 0 ? 0 : -1;
    }

    /// <summary>
    /// Returns true if the HexNode is an inverse one (the ones with negative power)
    /// </summary>
    /// <returns>True if the HexNode is an inverse one (the ones with negative power)</returns>
    public bool IsInverse()
    {
        return Type == HexType.RectangleInverse || Type == HexType.TriangleInverse ||
            Type == HexType.CircleInverse || Type == HexType.RectangleInverse2 ||
            Type == HexType.TriangleInverse2 || Type == HexType.DirectionalInverse2 || Type == HexType.DirectionalInverse;
    }
}

/// <summary>
/// Direction the HexNode is pointing at on the HexGrid
/// </summary>
public enum HexDirection
{
    Top,
    TopRight,
    BottomRight,
    Bottom,
    BottomLeft,
    TopLeft
}

/// <summary>
/// State of the HexNode
/// </summary>
public enum HexState
{
    Unpowered,
    Powered
}

/// <summary>
/// Type of the HexNode, basically its Power behavior
/// </summary>
public enum HexType
{
    Circle,
    Triangle,
    CircleInverse,
    TriangleInverse,
    Blank,
    Path,
    Target,
    Rectangle,
    RectangleInverse,
    Rectangle2,
    RectangleInverse2,
    Triangle2,
    TriangleInverse2,
    Directional2,
    DirectionalInverse2,
    Directional,
    DirectionalInverse
}