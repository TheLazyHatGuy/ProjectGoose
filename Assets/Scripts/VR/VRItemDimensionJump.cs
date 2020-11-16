﻿using UnityEngine;
using Valve.VR.InteractionSystem;

/// <summary>
/// Author: Cameron Scholes
/// This will switch between two objects depending on whether the player is in the future or the past
/// </summary>

[RequireComponent( typeof( Interactable ) )]
public class VRItemDimensionJump : MonoBehaviour
{
    [Tooltip("The item to be rendered in the present")]
    public GameObject normalObject;
    
    [Tooltip("The iterm to be rendered in the past")]
    public GameObject agedObject;

    [Tooltip("Enable if the item starts in the past")]
    public bool inFuture;
    
    [Tooltip("If enabled, the item will teleport back to its original location when detached from from the playes ahdn")]
    public bool teleportBackToOrigin;
    
    private Interactable interactable;
    private bool attached;
    private Vector3 oldPosition;
    private Quaternion oldRotation;
    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags 
                                                   & ~Hand.AttachmentFlags.SnapOnAttach
                                                   & ~Hand.AttachmentFlags.DetachOthers
                                                   & ~Hand.AttachmentFlags.VelocityMovement;
    
    void Awake()
    {
        interactable = this.GetComponent<Interactable>();
        EventManager.instance.OnTimeJump += TimeJump;
        SetActiveObject();
    }

    /// <summary>
    /// OnTimeJump event listener
    /// Only changes object state if the object is being held
    /// </summary>
    private void TimeJump()
    {
        // If the current object isn't attached to a hand, return, not currently being moved
        if (!attached) return;
        
        inFuture = !inFuture;
        SetActiveObject();
    }

    /// <summary>
    /// Swaps the active object depending on the given state
    /// </summary>
    private void SetActiveObject() {
        if (!inFuture) {
            agedObject.SetActive(false);
            normalObject.SetActive(true);
        } else {
            agedObject.SetActive(true);
            normalObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Called every Update() while a Hand is hovering over this object
    /// This is the object attachment function
    /// </summary>
    /// <param name="hand">The hand that is currently hovering over the object</param>
    private void HandHoverUpdate( Hand hand )
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            if (teleportBackToOrigin)
            {
                Transform transform1 = transform;
                
                // Save our position/rotation so that we can restore it when we detach
                oldPosition = transform1.position;
                oldRotation = transform1.rotation;
            }

            // Call this to continue receiving HandHoverUpdate messages,
            // and prevent the hand from hovering over anything else
            hand.HoverLock(interactable);

            // Attach this object to the hand
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
            attached = true;
        }
        else if (isGrabEnding)
        {
            // Detach this object from the hand
            hand.DetachObject(gameObject);
            attached = false;

            // Call this to undo HoverLock
            hand.HoverUnlock(interactable);

            if (teleportBackToOrigin)
            {
                // Restore position/rotation
                transform.position = oldPosition;
                transform.rotation = oldRotation;
            }
        }
    }
}
