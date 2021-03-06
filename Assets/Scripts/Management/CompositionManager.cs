﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Tomas
/// Manages the composition of the vials.
/// Changes it based on the time shifting and mixing of other chemicals
/// </summary>
public class CompositionManager : MonoBehaviour
{
    public Color currentColor = Color.white;
    public static Color minCompositiionColor = new Color(0.8f,0,0.3f);
    public static Color maxCompositionColor = new Color(1f,0.3f,0.6f);
    public GameObject puffEffect;
    public GameObject cloud;
    private Color previousColor = Color.white;
    private Material currentMaterial = null;
    private Valve.VR.InteractionSystem.Interactable interactable = null;
    public bool ISComposition = false;
    public bool HasElement = false;
    public bool debugHold = false;
    private void Start()
    {
        puffEffect.SetActive(false);

        //Get the interactable component
        interactable = GetComponentInParent<Valve.VR.InteractionSystem.Interactable>();

        //Allows access to material
        currentMaterial = GetComponent<Renderer>().material;

        //Make sure composition matches what its set to.
        currentMaterial.color = currentColor;

        //Save what it is currenty/ Wont trigger if they match later
        previousColor = currentColor;

        //Sets the timeSHift method to only call when time is jumped
        EventManager.instance.OnTimeJump += timeShiftChange;
    }

    public void mixChemical(Color chemicalAdditive)
    {
        
        if (chemicalAdditive != previousColor)
        {
            puffEffect.SetActive(false);
            //Mix the chemicals by 50%
            currentColor = Color.Lerp(chemicalAdditive,currentColor,0.5f);
            currentMaterial.color = currentColor;

            //Ensures only changed once else it will continuesly chnage the material until it becomes the additive
            previousColor = chemicalAdditive;
            puffEffect.SetActive(true);

            bool HasReachedGoal = detectIfWithinWinBounds();
            detectIfToxic(HasReachedGoal);

            //if(debugHold)
            //{
            //    Instantiate<GameObject>(cloud, this.transform.position, Quaternion.identity, transform);
            //}
        }
    }

    public void timeShiftChange()
    {
        //Only will trigger if attached to hand
        if(interactable.attachedToHand != null || debugHold)
        {
            //Values to modify
            float hue;
            float saturation;
            float brightness;

            //Get the Hue Saturation and Brightness
            Color.RGBToHSV(currentColor, out hue, out saturation, out brightness);

            //Modifying the hue by 0.5 will change its color to opposite
            hue += 0.5f;

            //Bring it back in range of 0-1
            hue = hue % 1f;

            //Recreate the Color
            Color changedColor = Color.HSVToRGB(hue, saturation, brightness);
            
            

            //Set as current
            currentMaterial.color = changedColor;
            currentColor = changedColor;
            //Detect if we got the mixture
            detectIfWithinWinBounds();
        }
    }

    private bool detectIfWithinWinBounds()
    {
        bool isWithinRange = false;

        //Check if All Within Range 
        bool isWithinRed = currentColor.r >= minCompositiionColor.r && currentColor.r <= maxCompositionColor.r;

        bool isWithinGreen = currentColor.g >= minCompositiionColor.g && currentColor.g <= maxCompositionColor.g;

        bool isWithinBlue = currentColor.b >= minCompositiionColor.b && currentColor.b <= maxCompositionColor.b;

        //If so return true and send event to declare we got it.
        if (isWithinRed && isWithinGreen && isWithinBlue)
        {
            isWithinRange = true;
            //Send Event
            ISComposition = true;
            Debug.Log("Composition Correct");
        }
        if(!isWithinRange && ISComposition)
        {
            ISComposition = false;
        }
        return isWithinRange;
    }

    private void detectIfToxic(bool isWinner)
    {
        if(!isWinner)
        {
            //If not a composition theres a chance it could be toxic if so generate the cloud
            int x = Random.Range(0, 1000);
            if(x % 4 == 0)
            {
                Instantiate<GameObject>(cloud, this.transform.position, Quaternion.identity);
            }
        }
    }

    public void callColorChange(GameObject otherVial)
    {
        //GameObject will be validated before this method is called
        //Take the second vial and mix it using the mix chemicalMethod.
        otherVial.GetComponent<CompositionManager>().mixChemical(currentColor);
    }

    public void IsPerfectCompositionDrunk()
    {
        if(ISComposition && HasElement)
        {
            EventManager.instance.Progress(STAGE.END);
            EventManager.instance.WinGame();
        }
    }

    void Update()
    {

    }
}
