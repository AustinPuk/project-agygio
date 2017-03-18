using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour {

    // Trying to implement a bezier pointer, based on VRTK pointer

    //TODO: CLEAN UP THIS CODE AND REMOVE REDUNDANT INFO

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

    private bool renderCurve = false;
    private Vector3 dest;
    private RaycastHit destinationHit = new RaycastHit();    
    protected Vector3 fixedForwardBeamForward;

    LayerMask terrainOnly;


    // Use this for initialization
    void Start () {
        terrainOnly = 1 << LayerMask.NameToLayer("Terrain");
        Ray raycast = new Ray(origin.position, Vector3.down);
        RaycastHit floor;
        bool ray = Physics.Raycast(raycast, out floor, 1000.0f, terrainOnly);
        if(ray)
            this.transform.position = floor.point + playerHeight * Vector3.up;
    }
	
	// Update is called once per frame
	void Update () {
        if (VRControls.instance.rightHand.gripPressed && VRControls.instance.rightHand.triggerPressed)
        {
            Vector3 jointPosition = ForwardBeam();
            Vector3 downPosition = DownBeam(jointPosition);
            DisplayCurvedBeam(jointPosition, downPosition);
            dest = downPosition;
            renderCurve = true;      
        }		
        else if (renderCurve == true)
        {            
            transform.position = new Vector3(dest.x, dest.y + playerHeight, dest.z);
            renderCurve = false;
            curve.enabled = false;
        }
	}

    private Vector3 ForwardBeam()
    {                
        float calculatedLength = maxDistance;
        Vector3 useForward = origin.forward;

        fixedForwardBeamForward = origin.forward;

        var actualLength = calculatedLength;
        Ray pointerRaycast = new Ray(origin.position, useForward);

        RaycastHit collidedWith;
        var hasRayHit = Physics.Raycast(pointerRaycast, out collidedWith, calculatedLength);

        float contactDistance = 0.0f;

        //reset if beam not hitting or hitting new target
        if (!hasRayHit || (destinationHit.collider && destinationHit.collider != collidedWith.collider))
        {
            contactDistance = 0.0f;
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            contactDistance = collidedWith.distance;
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && contactDistance < calculatedLength)
        {
            actualLength = contactDistance;
        }
        
        return (pointerRaycast.GetPoint(actualLength) + (Vector3.up));
    }

    private Vector3 DownBeam(Vector3 jointPosition)
    {
        Vector3 downPosition = Vector3.zero;
        Ray projectedBeamDownRaycast = new Ray(jointPosition, Vector3.down);
        RaycastHit collidedWith;

        var downRayHit = Physics.Raycast(projectedBeamDownRaycast, out collidedWith, float.PositiveInfinity);

        if (!downRayHit || (destinationHit.collider && destinationHit.collider != collidedWith.collider))
        {
            if (destinationHit.collider != null)
            {
                //?
            }
            destinationHit = new RaycastHit();
            downPosition = projectedBeamDownRaycast.GetPoint(0f);
        }

        if (downRayHit)
        {
            downPosition = projectedBeamDownRaycast.GetPoint(collidedWith.distance);
            // ?
            destinationHit = collidedWith;
        }
        return downPosition;
    }

    private void DisplayCurvedBeam(Vector3 jointPosition, Vector3 downPosition)
    {
        Vector3[] beamPoints = new Vector3[]
        {
                origin.position,
                jointPosition,
                downPosition,      
        };

        curve.numPositions = curveDensity;

        for (int i = 0; i < curveDensity; i++)
        {
            float t = i * (1.0f / curveDensity);
            Vector3 vec1 = Vector3.Lerp(beamPoints[0], beamPoints[1], t);
            Vector3 vec2 = Vector3.Lerp(beamPoints[1], beamPoints[2], t);
            curve.SetPosition(i, Vector3.Lerp(vec1, vec2, t));
        }

        curve.enabled = true;
    }
}
