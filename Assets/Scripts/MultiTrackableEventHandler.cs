/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class MultiTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status m_PreviousStatus;
    protected TrackableBehaviour.Status m_NewStatus;

    public static GameObject currentTrackable;
    public static GameObject otherTrackable;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        currentTrackable = null;

        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        // Can be improved by breaking down into separate IFs?
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) 
            /* NOTE: we may want special logic to deal with extended vs multi targets:
                - if old target is used with EXTENDED, but new is TRACKED, we want to switch to new
            */
        {
            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            Debug.Log(newStatus + " + " + mTrackableBehaviour.TrackableName);

            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            Debug.Log(newStatus + " + " + mTrackableBehaviour.TrackableName);

            OnTrackingLost();
        }
        else
        {
            //Debug.Log(newStatus + " (else)");
            Debug.Log(newStatus + " + " + mTrackableBehaviour.TrackableName);
            

            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        string name = gameObject.name;
        //Debug.Log("Tracking found: " + name);
 
        if (currentTrackable == null) {
            currentTrackable = gameObject;
            Debug.Log("Current trackable set to: " + name + ". Current status is " + m_NewStatus);
            setChildEnabled(true);
        } 
        else {
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

    protected virtual void OnTrackingLost()
    {
        string name = gameObject.name;
        Debug.Log("Tracking lost: " + name + ". Current status is " + m_NewStatus);

        if (gameObject == currentTrackable) {
            if (otherTrackable != null) {
                switchOtherToCurrent();             
            } else {
                currentTrackable = null;
            }
        }
        else if (gameObject == otherTrackable) {
            otherTrackable = null;
        }
        setChildEnabled(false);
    }

 
    public void setChildEnabled(bool enabled) {
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

    private void switchOtherToCurrent() {
        otherTrackable.GetComponent<MultiTrackableEventHandler>().setChildEnabled(true);
        currentTrackable = otherTrackable;
        otherTrackable = null;   
    }

    #endregion // PROTECTED_METHODS
}