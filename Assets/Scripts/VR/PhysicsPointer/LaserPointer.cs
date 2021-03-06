﻿using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Author: Cameron Scholes
/// The laser pointer method of interacting with objects
/// </summary>

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    public bool enabled;
    
    // Single is the name Vector1
    public SteamVR_Action_Single startLaser = SteamVR_Input.GetSingleAction("StartLaser");
    public SteamVR_Action_Boolean pullObject = SteamVR_Input.GetBooleanAction("PullObject");
    
    public float defaultLength = 3.0f;

    private LineRenderer lineRenderer;
    private LaserPonterReciever lastHit;

    void Awake()
    {
        if (startLaser == null)
            Debug.LogError("LaserPointer is missing StartLaser.", this);
        
        if (pullObject == null)
            Debug.LogError("LaserPointer is missing PullObject.", this);

        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!enabled)
            return;
        
        if (startLaser.axis > 0.25f)
        {
            lineRenderer.enabled = true;
            UpdateLength();
        }
        else
        {
            lineRenderer.enabled = false;
            RayExit();
        }
    }

    private void UpdateLength()
    {
        // Calculate the start of the line render
        lineRenderer.SetPosition(0, transform.position);
        
        // Calculate the end of the line render
        lineRenderer.SetPosition(1, CalculateEnd());
    }
    
    private Vector3 CalculateEnd()
    {
        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition;

        if (hit.collider)
        {
            endPosition = hit.point;
            lastHit = hit.transform.GetComponent<LaserPonterReciever>(); // TODO: Is there a less expensive method

            if (lastHit != null)
                if (pullObject.state)
                    lastHit.Click(transform);
                else
                    lastHit.HitByRay();
        }
        else
        {
            endPosition = DefaultEnd(defaultLength);
            RayExit();
        }
        
        return endPosition;
    }

    private RaycastHit CreateForwardRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, defaultLength);
        
        return hit;
    }

    private Vector3 DefaultEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }

    private void RayExit()
    {
        if (!lastHit) return;
        
        lastHit.RayExit();
        lastHit = null;
    }
}
