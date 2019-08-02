using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class MultiTrackableEventHandler : DefaultTrackableEventHandler
{
    public static GameObject currentTrackable;
    public static GameObject otherTrackable;

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected override void Start()
    {
        currentTrackable = null;
        base.Start();
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PROTECTED_METHODS

    protected override void OnTrackingFound()
    {
        string name = gameObject.name;
        //Debug.Log("Tracking found: " + name);

        if (currentTrackable == null)
        {
            currentTrackable = gameObject;
            Debug.Log("Current trackable set to: " + name + ". Current status is " + m_NewStatus);
            setChildEnabled(true);
        }
        else
        {
            //NOTE: we may get this Found call even when this target was already tracked.
            //maybe related to how both TRACKED and EXTENDED_TRACKED result in call to here.
            if (gameObject != currentTrackable &&
                gameObject != otherTrackable)
            {
                Debug.Log($"Remember additional trackable for later {gameObject.name} - is tracking {currentTrackable.name} already");
                otherTrackable = gameObject;
            }

            // Is the currently tracked target on extended tracking and the target in queue isn't?
            if (otherTrackable != null)
            {
                if (currentTrackable.GetComponent<MultiTrackableEventHandler>().m_NewStatus == TrackableBehaviour.Status.EXTENDED_TRACKED
                && otherTrackable.GetComponent<MultiTrackableEventHandler>().m_NewStatus != TrackableBehaviour.Status.EXTENDED_TRACKED)
                {
                    Debug.Log("***Change target to " + gameObject.name + "***");
                    currentTrackable.GetComponent<MultiTrackableEventHandler>().setChildEnabled(false);
                    switchOtherToCurrent();
                }
            }
        }
    }

    protected override void OnTrackingLost()
    {
        string name = gameObject.name;
        Debug.Log("Tracking lost: " + name + ". Current status is " + m_NewStatus);

        if (gameObject == currentTrackable)
        {
            if (otherTrackable != null)
            {
                switchOtherToCurrent();
            }
            else
            {
                currentTrackable = null;
            }
        }
        else if (gameObject == otherTrackable)
        {
            otherTrackable = null;
        }
        setChildEnabled(false);
    }

    //adapted from DefaultTrackableEventHandler OnTrackingFound & *Lost, avoiding copy-paste of bool setting there
    private void setChildEnabled(bool enabled)
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = enabled;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = enabled;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = enabled;
    }

    private void switchOtherToCurrent()
    {
        otherTrackable.GetComponent<MultiTrackableEventHandler>().setChildEnabled(true);
        currentTrackable = otherTrackable;
        otherTrackable = null;
    }

    #endregion // PROTECTED_METHODS
}