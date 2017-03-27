using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour {

    // Trying to implement a bezier pointer, based on VRTK pointer

    //TODO: CLEAN UP THIS CODE AND REMOVE REDUNDANT INFO

    [SerializeField]
    private PlayerHand hand;

    [SerializeField]
    private Transform origin;

    [SerializeField]
    private float playerHeight = 1.0f;
    [SerializeField]
    private float maxDistance = 10.0f;
    [SerializeField]
    private int curveDensity = 10;

    [SerializeField]
    private LineRenderer curve;

    [SerializeField]
    private Color yesColor;

    [SerializeField]
    private Color noColor;

    [SerializeField]
    LayerMask teleportLayers;

    private bool renderCurve = false;
    private bool destValid;
    private Vector3 dest;    
    protected Vector3 fixedForwardBeamForward;    

    // Use this for initialization
    void Start () {
        Ray raycast = new Ray(origin.position, Vector3.down);
        RaycastHit floor;
        bool ray = Physics.Raycast(raycast, out floor, 1000.0f, teleportLayers);
        if(ray)
            this.transform.position = floor.point + playerHeight * Vector3.up;
    }
	
	// Update is called once per frame
	void Update () {

        // "Shooting Gun Pose" with hand to activate teleport

        if (hand.padPressed && !hand.triggerPressed)
        {
            Vector3 jointPosition = ForwardBeam();
            if (destValid)
            {
                if (DownBeam(jointPosition))
                {
                    DisplayCurvedBeam(jointPosition, dest);
                    destValid = true;
                    renderCurve = true;
                    ChangeColor(yesColor);
                }
                else
                {
                    destValid = false;
                    renderCurve = false;
                    curve.enabled = false;
                    ChangeColor(noColor);
                }
            }
            else
            {
                DownBeam(jointPosition);
                DisplayCurvedBeam(jointPosition, dest);
                ChangeColor(noColor);
                renderCurve = true;                
            }
        }
        else if (hand.padPressed && hand.triggerPressed && renderCurve == true)
        {
            if (destValid)
                StartCoroutine(FlashStep(new Vector3(dest.x, dest.y + playerHeight, dest.z), 0.1f));
                //transform.position = new Vector3(dest.x, dest.y + playerHeight, dest.z);
            renderCurve = false;
            curve.enabled = false;
        }
        else
        {
            renderCurve = false;
            curve.enabled = false;
        }
	}

    private Vector3 ForwardBeam()
    {                
        float calculatedLength = maxDistance;
        Vector3 useForward = origin.forward;

        fixedForwardBeamForward = origin.forward;        
        Ray pointerRaycast = new Ray(origin.position, useForward);

        RaycastHit collidedWith;
        var hasRayHit = Physics.Raycast(pointerRaycast, out collidedWith, calculatedLength, teleportLayers);

        float contactDistance = 0.0f;
        destValid = true;

        //check if beam has hit a new target
        if (hasRayHit)
        {
            contactDistance = collidedWith.distance;
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && contactDistance < calculatedLength)
        {
            calculatedLength = contactDistance;

            //Debug.Log("Ray Collided With " + collidedWith.collider.name);

            if (collidedWith.collider.gameObject.tag != "Terrain")
            {
                destValid = false;
                ChangeColor(noColor);
            }
        }        
        
        return (pointerRaycast.GetPoint(calculatedLength) + (Vector3.up));
    }

    private bool DownBeam(Vector3 jointPosition)
    {
        Ray projectedBeamDownRaycast = new Ray(jointPosition, Vector3.down);
        RaycastHit collidedWith;

        var downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith, float.PositiveInfinity, teleportLayers);


        dest = projectedBeamDownRaycast.GetPoint(collidedWith.distance);        

        if (downRayHit && collidedWith.collider.tag == "Terrain")       
            return true;
        else
            return false;
    }

    private void DisplayCurvedBeam(Vector3 jointPosition, Vector3 downPosition)
    {
        Vector3[] beamPoints = new Vector3[]
        {
                origin.position,
                jointPosition,
                downPosition,      
        };

        curve.numPositions = curveDensity + 1;

        for (int i = 0; i <= curveDensity; i++)
        {
            float t = i * (1.0f / curveDensity);
            Vector3 vec1 = Vector3.Lerp(beamPoints[0], beamPoints[1], t);
            Vector3 vec2 = Vector3.Lerp(beamPoints[1], beamPoints[2], t);
            curve.SetPosition(i, Vector3.Lerp(vec1, vec2, t));
        }

        curve.enabled = true;
    }

    private void ChangeColor(Color color)
    {
        curve.GetComponent<Renderer>().material.color = color;
    }

    private IEnumerator FlashStep(Vector3 destination, float time)
    {
        for (int i = 0; i <= 20; i++)
        {
            transform.position = Vector3.Lerp(transform.position, destination, (i / 20.0f));

            yield return new WaitForSeconds(time / 20.0f);
        }        
    }
}
