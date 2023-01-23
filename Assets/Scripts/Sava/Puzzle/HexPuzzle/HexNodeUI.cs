using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the UI representation of a HexNode for the Puzzle
/// </summary>
public class HexNodeUI : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Time it takes to rotate the hexagonal node
    /// </summary>
    private float rotationDuration = 0.2f;

    /// <summary>
    /// Reference to the RectTransform of the node
    /// </summary>
    private RectTransform rt;

    /// <summary>
    /// Reference to the Image of the node
    /// </summary>
    private Image image;

    /// <summary>
    /// Stores the rotation of the RectTransform
    /// </summary>
    private Quaternion currentRotation;

    /// <summary>
    /// Is the node rotating?
    /// </summary>
    bool rotating = false;

    /// <summary>
    /// Reference to the HexNode this HexNodeUI is representing
    /// </summary>
    public HexNode hexNode { get; private set; }

    /// <summary>
    /// Reference to the HexPuzzleController this node is part of
    /// </summary>
    HexagonalPuzzle hexPuzzleController;

    /// <summary>
    /// Reference to the TextMesh containing the target power for this node
    /// </summary>
    public TextMeshProUGUI targetText;

    /// <summary>
    /// Reference to the TextMesh containing the current power for this node
    /// </summary>
    public TextMeshProUGUI pathText;

    /// <summary>
    /// Reference to the Sprite this node has to use
    /// </summary>
    private HexGraphicsScriptableObject hexGraphicsRef;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        currentRotation = rt.localRotation;
    }

    /// <summary>
    /// Clicking a node, it has to rotate or switch the power, depending on left or right click
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (hexPuzzleController.PuzzleCompleted) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RotateHex();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            SwitchHex();
        }
    }

    /// <summary>
    /// Rotates the UI of the node counterclockwise
    /// </summary>
    public void RotateHex()
    {
        if (rotating) return;
        if (hexNode.Type != HexType.Path && hexNode.Type != HexType.Target)
        {
            StartCoroutine(Rotate());
            hexPuzzleController.RotateHex(hexNode);
        }
    }

    /// <summary>
    /// Notifies the puzzle controller to turn the power on/off for the node
    /// </summary>
    public void SwitchHex()
    {
        hexPuzzleController.SwitchHex(hexNode);
    }

    /// <summary>
    /// Turns the power on/off for the node
    /// </summary>
    public void SwitchPower()
    {
        if (hexNode.Type != HexType.Target)
        {
            if (hexNode.PowerCounter > 0)
                image.sprite = hexGraphicsRef.PositiveGraphics;
            else if (hexNode.PowerCounter < 0)
                image.sprite = hexGraphicsRef.NegativeGraphics;
            else
                image.sprite = hexGraphicsRef.NeutralGraphics;

            if (hexNode.Type == HexType.Path)
            {
                if (hexNode.PowerCounter != 0)
                    pathText.text = hexNode.PowerCounter.ToString();
                else
                    pathText.text = "";
            }
        }
    }

    /// <summary>
    /// Updates the Sprite of the node relative to the power it has
    /// </summary>
    /// <param name="power">New power for the node</param>
    /// <returns>True if the new power for the node is equal to its target power</returns>
    public bool UpdateTargetPower(int power)
    {
        if (hexNode.Type != HexType.Target) return false;
        hexNode.PowerCounter = power;

        targetText.text = hexNode.TargetPower.ToString();
        if (hexNode.PowerCounter == hexNode.TargetPower)
        {
            image.sprite = hexGraphicsRef.SpecialGraphics1;
            targetText.text = "";
        }
        else if (hexNode.PowerCounter > 0 && hexNode.PowerCounter < hexNode.TargetPower)
            image.sprite = hexGraphicsRef.PositiveGraphics;
        else if (hexNode.PowerCounter > hexNode.TargetPower)
            image.sprite = hexGraphicsRef.SpecialGraphics2;
        else if (hexNode.PowerCounter == 0)
            image.sprite = hexGraphicsRef.NeutralGraphics;

        if (hexNode.PowerCounter == hexNode.TargetPower)
            return true;
        return false;
    }

    /// <summary>
    /// Initializes the Sprite and power counters for the node
    /// </summary>
    /// <param name="controllerRef">Refrence to the puzzle controller this node is part of</param>
    /// <param name="hexNodeRef">Refrence to the HexNode this HexNodeUI is representing</param>
    /// <param name="hexScriptableObjectRef">Reference to the Sprites this HexNodeUI will use</param>
    public void Initialize(HexagonalPuzzle controllerRef, HexNode hexNodeRef, HexGraphicsScriptableObject hexScriptableObjectRef)
    {
        hexGraphicsRef = hexScriptableObjectRef;
        hexPuzzleController = controllerRef;
        hexNode = hexNodeRef;
        if (hexNode.Type == HexType.Path)
        {
            pathText.gameObject.SetActive(true);
        }
        else if (hexNode.Type == HexType.Target)
        {
            targetText.gameObject.SetActive(true);
            targetText.text = hexNode.TargetPower.ToString();
        }
    }

    /// <summary>
    /// Interpolates the node rotation
    /// </summary>
    /// <returns></returns>
    IEnumerator Rotate()
    {
        rotating = true;
        float transitionTime = 0f;
        float value;
        while (transitionTime < rotationDuration)
        {
            transitionTime += Time.deltaTime;
            value = Mathf.SmoothStep(0, 1f, transitionTime / rotationDuration);
            rt.localRotation = Quaternion.LerpUnclamped(rt.localRotation, currentRotation * Quaternion.Euler(Vector3.forward * 60f), value);
            yield return null;
        }
        currentRotation = rt.localRotation;
        rotating = false;
    }
}
