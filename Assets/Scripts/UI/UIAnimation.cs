using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Provides "drag and drop" animation, with a series of simple animation patterns that can be configured
/// </summary>
public class UIAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    /// <summary>
    /// List of all the UITransition information configured for the current GameObject
    /// </summary>
    [SerializeField]
    private List<UITransition> uITransitions = new List<UITransition>();

    /// <summary>
    /// Reference to the RectTransform this MonoBehaviour is attached to
    /// </summary>
    private RectTransform baseRectTransform;

    /// <summary>
    /// Reference to the text component attached to the RectTransform this MonoBehaviour is attached to
    /// </summary>
    private TextMeshProUGUI baseText;

    /// <summary>
    /// Reference to the Image component attached to the RectTransform this MonoBehaviour is attached to
    /// </summary>
    private Image baseImage;

    /// <summary>
    /// Base scale of the RectTransform this MonoBehaviour is attached to
    /// </summary>
    private Vector3 baseScale;

    /// <summary>
    /// Unused
    /// </summary>
    private Vector3 startTransitionScale;

    /// <summary>
    /// Start position of the RectTransform this MonoBehaviour is attached to
    /// </summary>
    private Vector3 startTransitionPosition;

    /// <summary>
    /// Unused
    /// </summary>
    private Vector3 basePosition;

    /// <summary>
    /// Start rotation of the RectTransform this MonoBehaviour is attached to
    /// </summary>
    private Quaternion baseRotation;

    /// <summary>
    /// Unused
    /// </summary>
    private Quaternion currentRotation;

    /// <summary>
    /// Base color of the TextMesh this MonoBehaviour is attached to
    /// </summary>
    private Color baseTextColor;

    /// <summary>
    /// Base image color of the Image this MonoBehaviour is attached to
    /// </summary>
    private Color baseImageColor;

    /// <summary>
    /// Was a trigger being activated? Needs a refactory, does not work for multiple triggers
    /// </summary>
    private bool triggered = false;

    /// <summary>
    /// Was this ui left clicked? Used in case of reverse animation for click events
    /// </summary>
    private bool clicked = false;

    /// <summary>
    /// Was this ui right clicked? Used in case of reverse animation for click events
    /// </summary>
    private bool clickedRight = false;

    /// <summary>
    /// Coroutines associated to hover enter events
    /// </summary>
    private List<Coroutine> hoverEnterCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to hover exit events
    /// </summary>
    private List<Coroutine> hoverExitCoroutines = new List<Coroutine>();

    /// <summary>
    /// Unused
    /// </summary>
    private List<Coroutine> triggerEnterCoroutines = new List<Coroutine>();

    /// <summary>
    /// Unused
    /// </summary>
    private List<Coroutine> triggerExitCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to left click events
    /// </summary>
    private List<Coroutine> clickEnterCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to left click reverse events
    /// </summary>
    private List<Coroutine> clickExitCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to right click events
    /// </summary>
    private List<Coroutine> clickRightEnterCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to right click reverse events
    /// </summary>
    private List<Coroutine> clickRightExitCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to enable events
    /// </summary>
    private List<Coroutine> enableCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to disable events
    /// </summary>
    private List<Coroutine> disableCoroutines = new List<Coroutine>();

    /// <summary>
    /// Unused
    /// </summary>
    private List<Coroutine> destroyCoroutines = new List<Coroutine>();

    /// <summary>
    /// Coroutines associated to set parent events
    /// </summary>
    private List<Coroutine> parentChangedCoroutines = new List<Coroutine>();

    /// <summary>
    /// Number of active coroutines for click events
    /// </summary>
    int clickCoroutines = 0;

    /// <summary>
    /// Unused
    /// </summary>
    int clickRightCoroutines = 0;

    /// <summary>
    /// Action to execute at the end of the "trigger1" animation
    /// </summary>
    Action trigger1Function;

    /// <summary>
    /// Action to execute at the end of the "trigger2" animation
    /// </summary>
    Action trigger2Function;

    /// <summary>
    /// Action to execute at the end of the "trigger3" animation
    /// </summary>
    Action trigger3Function;

    /// <summary>
    /// Action to execute at the end of the "disable" animation
    /// </summary>
    Action disableFunction;

    /// <summary>
    /// Action to execute at the end of the "click" animation
    /// </summary>
    Action<PointerEventData> clickFunction;

    Vector3 baseRot;
    Vector3 currRot;

    private void Awake() {
        baseRectTransform = GetComponent<RectTransform>();
        baseText = GetComponent<TextMeshProUGUI>();
        if (baseText != null)
            baseTextColor = baseText.color;
        baseImage = GetComponent<Image>();
        if (baseImage != null)
            baseImageColor = baseImage.color;
        baseScale = baseRectTransform.localScale;
        basePosition = baseRectTransform.anchoredPosition;
        baseRotation = baseRectTransform.localRotation;
        currentRotation = baseRotation;
    }

    public void SetClickAction(Action<PointerEventData> action) {
        clickFunction = action;
    }

    public void TriggerAnimation(int triggerIndex, Action notifyComplete = null) {

        if (triggerIndex == 1) {
            trigger1Function = notifyComplete;
        }
        else if (triggerIndex == 2) {
            trigger2Function = notifyComplete;
        }
        else if (triggerIndex == 3) {
            trigger3Function = notifyComplete;
        }

        foreach (var transition in uITransitions) {
            if ((triggerIndex == 1 && transition.TransitionEvent == UITransitionEvent.Trigger1) ||
                (triggerIndex == 2 && transition.TransitionEvent == UITransitionEvent.Trigger2) ||
                (triggerIndex == 3 && transition.TransitionEvent == UITransitionEvent.Trigger3)) {
                Coroutine c = StartCoroutine(Transition(transition, false));
                triggered = !triggered;
            }
        }
    }

    public void DisableWithAnimation(Action disableMethod = null) {
        disableFunction = disableMethod;
        foreach (var transition in uITransitions) {
            if (transition.TransitionEvent == UITransitionEvent.Disable) {
                Coroutine c = StartCoroutine(Transition(transition, false, UIEndTransitionAction.Disable));
                disableCoroutines.Add(c);
                for (int i = 0; i < enableCoroutines.Count; i++)
                    StopCoroutine(enableCoroutines[i]);
                enableCoroutines.Clear();

                for (int i = 0; i < parentChangedCoroutines.Count; i++)
                    StopCoroutine(parentChangedCoroutines[i]);
                parentChangedCoroutines.Clear();
            }
        }
    }

    public void DestroyWithAnimation() {
        foreach (var transition in uITransitions) {
            if (transition.TransitionEvent == UITransitionEvent.Destroy) {
                Coroutine c = StartCoroutine(Transition(transition, false, UIEndTransitionAction.Destroy));

                for (int i = 0; i < parentChangedCoroutines.Count; i++)
                    StopCoroutine(parentChangedCoroutines[i]);
                parentChangedCoroutines.Clear();
            }
        }
    }

    private void OnEnable() {
        foreach (var transition in uITransitions) {
            if (transition.TransitionEvent == UITransitionEvent.Enable) {
                Coroutine c = StartCoroutine(Transition(transition, false));
                enableCoroutines.Add(c);
                for (int i = 0; i < disableCoroutines.Count; i++)
                    StopCoroutine(disableCoroutines[i]);
                disableCoroutines.Clear();
            }
        }
    }

    private void OnTransformParentChanged() {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(ParentChanged());
        
    }

    IEnumerator ParentChanged() {
        yield return new WaitForEndOfFrame();

        basePosition = baseRectTransform.anchoredPosition;

        foreach (var transition in uITransitions) {
            if (transition.TransitionEvent == UITransitionEvent.SetParent) {
                Coroutine c = StartCoroutine(Transition(transition, false));
                parentChangedCoroutines.Add(c);
            }
        }
    }

    private void OnDisable() {
        baseRectTransform.localScale = baseScale;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        for (int i = 0; i < hoverExitCoroutines.Count; i++)
            StopCoroutine(hoverExitCoroutines[i]);
        hoverExitCoroutines.Clear();

        foreach (var transition in uITransitions) {
            if (transition.TransitionEvent == UITransitionEvent.Hover) {
                Coroutine c = StartCoroutine(Transition(transition, false));
                hoverEnterCoroutines.Add(c);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        for (int i = 0; i < hoverEnterCoroutines.Count; i++)
            StopCoroutine(hoverEnterCoroutines[i]);
        hoverEnterCoroutines.Clear();

        foreach (var transition in uITransitions) {
            if (transition.TransitionEvent == UITransitionEvent.Hover && transition.ReplayForOpposite) {
                Coroutine c = StartCoroutine(Transition(transition, true));
                hoverExitCoroutines.Add(c);
            }
        }
    }

    IEnumerator Transition(UITransition transition, bool reverseTransition, UIEndTransitionAction endAction = UIEndTransitionAction.None) {
        float transitionDuration = transition.TransitionDuration +Mathf.Epsilon;
        float transitionTime = 0f;
        startTransitionScale = baseRectTransform.localScale;
        startTransitionPosition = baseRectTransform.anchoredPosition;

        while (transitionTime < transitionDuration) {
            transitionTime = Mathf.Clamp( transitionTime + Time.deltaTime, 0f, transitionDuration);
            CalculateTransition(transition, transitionTime, reverseTransition ? 1f : 0f, reverseTransition);
            yield return null;
        }

        HandleCleanup(transition.TransitionEvent);

        if (transition.TransitionType == UITransitionType.Rotation)
            currentRotation = baseRectTransform.localRotation;

        if (endAction == UIEndTransitionAction.Disable) {
            gameObject.SetActive(false);
        }
        else if (endAction == UIEndTransitionAction.Destroy) {
            Destroy(gameObject);
        }
    }

    private void CalculateTransition(UITransition transition, float time, float reverseTimeFactor, bool reverseTransition = false) {
        float value = transition.TransitionCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(reverseTimeFactor - time / transition.TransitionDuration)));
        Color color;
        switch (transition.TransitionType) {
            case UITransitionType.Scale:
                value *= transition.TransitionScale;
                baseRectTransform.localScale = baseScale * value;
                break;
            case UITransitionType.TextCharacterSpacing:
                value *= transition.TransitionScale;
                if (baseText is not null) {
                    baseText.characterSpacing = value;
                }
                break;
            case UITransitionType.TextUnderline:
                value *= transition.TransitionScale;
                if (baseText is not null) {
                    if (reverseTimeFactor == 0f)
                        baseText.fontStyle |= FontStyles.Underline;
                    else
                        baseText.fontStyle ^= FontStyles.Underline;
                }
                break;
            case UITransitionType.Movement:
                baseRectTransform.anchoredPosition = Vector3.LerpUnclamped(startTransitionPosition, transition.Vector3Parameter, value);
                break;
            case UITransitionType.Rotation:
                baseRot = baseRotation.eulerAngles;
                currRot = Vector3.Lerp(baseRot, transition.Vector3Parameter, value);
                baseRectTransform.localRotation = Quaternion.Euler(currRot);
                break;
            case UITransitionType.TextColor:
                color = Color.Lerp(baseTextColor, transition.ColorParameter, value);
                if (baseText is not null) {
                    baseText.color = color;
                }
                break;
            case UITransitionType.SpriteColor:
                color = Color.Lerp(baseImageColor, transition.ColorParameter, value);

                if (baseImage is not null) {
                    baseImage.color = color;
                }
                break;
            case UITransitionType.Audio:
                AudioManager.Instance.PlaySFX(transition.StringParameter);
                break;
        }
    }

    private void HandleCleanup(UITransitionEvent uITransitionEvent) {
        if (uITransitionEvent == UITransitionEvent.Trigger1) {
            if (trigger1Function != null) {
                trigger1Function.Invoke();
                trigger1Function = null;
            }
        }
        else if (uITransitionEvent == UITransitionEvent.Trigger2) {
            if (trigger2Function != null) {
                trigger2Function.Invoke();
                trigger2Function = null;
            }
        }
        else if (uITransitionEvent == UITransitionEvent.Trigger3) {
            if (trigger3Function != null) {
                trigger3Function.Invoke();
                trigger3Function = null;
            }
        }
        else if (uITransitionEvent == UITransitionEvent.Disable) {
            if (disableFunction != null) {
                disableFunction.Invoke();
                disableFunction = null;
            }
        }
        else if (uITransitionEvent == UITransitionEvent.Click) {
            clickCoroutines--;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (clickCoroutines > 0)
            return;
        foreach (var transition in uITransitions) {
            if (transition.TransitionEvent == UITransitionEvent.Click || transition.TransitionEvent == UITransitionEvent.RightClick) {
                if (transition.TransitionEvent == UITransitionEvent.Click && eventData.button == PointerEventData.InputButton.Left)
                    HandleLeftClick(transition);
                else if (transition.TransitionEvent == UITransitionEvent.RightClick && eventData.button == PointerEventData.InputButton.Right)
                    HandleRightClick(transition);
            }
        }
    }

    private void HandleLeftClick(UITransition transition) {
        clickCoroutines++;
        Coroutine c = StartCoroutine(Transition(transition, clicked && transition.ReplayForOpposite));

        if (transition.ReplayForOpposite) {

            if (!clicked) {
                clickEnterCoroutines.Add(c);
                for (int i = 0; i < clickExitCoroutines.Count; i++)
                    StopCoroutine(clickExitCoroutines[i]);
                clickExitCoroutines.Clear();
            }
            else {
                clickExitCoroutines.Add(c);
                for (int i = 0; i < clickEnterCoroutines.Count; i++)
                    StopCoroutine(clickEnterCoroutines[i]);
                clickEnterCoroutines.Clear();
            }
        }
        else {
            for (int i = 0; i < clickExitCoroutines.Count; i++)
                StopCoroutine(clickExitCoroutines[i]);
            clickExitCoroutines.Clear();
            clickEnterCoroutines.Add(c);
        }
            clicked = !clicked;
    }

    private void HandleRightClick(UITransition transition) {
        clickRightCoroutines++;
        Coroutine c = StartCoroutine(Transition(transition, clickedRight && transition.ReplayForOpposite));
        if (!clickedRight) {
            clickRightEnterCoroutines.Add(c);
            for (int i = 0; i < clickRightExitCoroutines.Count; i++)
                StopCoroutine(clickExitCoroutines[i]);
            clickRightExitCoroutines.Clear();
        }
        else {
            clickRightExitCoroutines.Add(c);
            for (int i = 0; i < clickRightEnterCoroutines.Count; i++)
                StopCoroutine(clickRightEnterCoroutines[i]);
            clickRightEnterCoroutines.Clear();
        }
        clickedRight = !clickedRight;
    }
}