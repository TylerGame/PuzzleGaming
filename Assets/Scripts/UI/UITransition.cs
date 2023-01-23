using System;
using UnityEngine;

/// <summary>
/// Parameters collection to define a UI animation transition
/// </summary>
[Serializable]
public class UITransition {
    /// <summary>
    /// Type of transition
    /// </summary>
    public UITransitionType TransitionType;

    /// <summary>
    /// Event triggering the transition
    /// </summary>
    public UITransitionEvent TransitionEvent;

    /// <summary>
    /// Curve used to interpolate the transition
    /// </summary>
    public AnimationCurve TransitionCurve;

    /// <summary>
    /// Is the transition a looping one?
    /// </summary>
    public bool Loop;

    /// <summary>
    /// Should the animation be replayed when the "opposite" transition event takes place?
    /// </summary>
    public bool ReplayForOpposite;

    /// <summary>
    /// Duration of the transition
    /// </summary>
    public float TransitionDuration = 0.25f;

    /// <summary>
    /// Scale of the transition
    /// </summary>
    public float TransitionScale = 1f;

    /// <summary>
    /// Float parameter used for transitions that need it
    /// </summary>
    public float FloatParameter = 0f;

    /// <summary>
    /// Vector3 parameter used for transitions that need it
    /// </summary>
    public Vector3 Vector3Parameter = Vector3.zero;

    /// <summary>
    /// Color parameter used for transitions that need it
    /// </summary>
    public Color ColorParameter = Color.black;

    /// <summary>
    /// String parameter used for transitions that need it
    /// </summary>
    public string StringParameter;
}

/// <summary>
/// Type of transition
/// </summary>
public enum UITransitionType {
    Scale,
    Movement,
    Rotation,
    Audio,
    TextCharacterSpacing,
    TextUnderline,
    TextColor,
    SpriteColor
}

/// <summary>
/// Event that triggers a transition
/// </summary>
public enum UITransitionEvent {
    Hover,
    Click,
    RightClick,
    Enable,
    Disable,
    Destroy,
    SetParent,
    Trigger1,
    Trigger2,
    Trigger3
}

/// <summary>
/// Action to perform at the end of a transition
/// </summary>
enum UIEndTransitionAction {
    None,
    Disable,
    Destroy
}