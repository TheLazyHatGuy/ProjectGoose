﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CabinetDrawer : MonoBehaviour
{
    private bool holdingHandle;
    private Vector3 cross;
    private const float Multiplier = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (holdingHandle)
            GetComponent<Rigidbody>().velocity = cross * Multiplier;
    }

    protected virtual void HandOverUpdate(Hand hand)
    {
        GrabTypes grab = hand.GetGrabStarting();

        if(grab != GrabTypes.None)
        {
            holdingHandle = true;
            cross = CalculatePullForce(hand);
        }
        else
        {
            holdingHandle = false;
        }
    }

    private Vector3 CalculatePullForce(Hand hand)
    {
        Vector3 pullDirection = hand.transform.position - transform.position;

        //Ignore y and x so the drawer moves in a straight line
        pullDirection.y = 0;
        pullDirection.x = 0;

        Vector3 force = hand.transform.position - transform.position;

        return Vector3.Cross(pullDirection, force);
    }
}