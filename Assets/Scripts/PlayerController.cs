using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    FileManager file_manager;

    [SerializeField]
    GameObject miniature;

    [SerializeField]
    Transform finger_tip;

    [SerializeField]
    float rotation_step = 5.0f;

    LineRenderer line;

    bool grip_down;

    //Trigger Variabls
    float trigger_timer;
    [SerializeField]
    float trigger_hold_delay;

    List<Item> selected_items;

    void Awake()
    {
        line = this.GetComponent<LineRenderer>();
    }

    void Start()
    {
        selected_items = new List<Item>();
        line.enabled = false;
        line.useWorldSpace = true;

        grip_down = false;
    }
   
    void Update()
    {
        //Grip toggles pointer, and allows for selection
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
        {
            //Debug.Log("PC: Right Hand");            
            grip_down = true;
            pointer();
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
        {
            //Debug.Log("PC: Grip Release");
            line.enabled = false;
            grip_down = false;

            foreach (Item item in selected_items)
                item.deselect();

            selected_items.Clear();
        }

        if (grip_down)
        {
            //Index Trigger: Short click for selecting, hold for grab and move.
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                //Debug.Log("PC: Trigger Select");
                //if (trigger_timer > 0.0f)
                    //check_select();
                trigger_timer = trigger_hold_delay;
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                check_select();
                trigger_timer = trigger_hold_delay;
            }
            else if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            {
                if (trigger_timer > 0.0f) trigger_timer -= Time.deltaTime;               

                if (trigger_timer <= 0.0f)
                {
                    //Debug.Log("PC: Trigger Grab");

                    if (selected_items.Count > 0)
                    {
                        // Add Joysticks for grabbed object controls
                        Vector2 joystick = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

                        foreach (Item item in selected_items)
                        {
                            Vector3 rot = new Vector3(0.0f, 0.0f, 0.0f);

                            //Rotations on X Axis
                            if (joystick.x > 0.5f)
                            {
                                rot = new Vector3(0.0f, 0.0f, rotation_step);
                            }
                            else if (joystick.x < -0.5f)
                            {
                                rot = new Vector3(0.0f, 0.0f, -rotation_step);
                            }

                            float dist_offset = 0.0f;

                            if (joystick.y > 0.5f)
                            {
                                dist_offset = 0.1f;
                            }
                            else if (joystick.y < -0.5f)
                            {
                                dist_offset = -0.1f;
                            }

                            item.move(finger_tip, dist_offset, rot);
                        }

                    }
                }      
            }            

            if (selected_items.Count <= 0 && OVRInput.GetDown(OVRInput.RawButton.A))
            {
                //If Press "A" Button, Teleport. Can't teleport with same hand when holding objects
                check_teleport();
            }
        }

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            file_manager.RecordData();
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            file_manager.LoadData();
        }

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {
            //Toggles Miniature
            miniature.SetActive(!miniature.activeSelf);
        }


    }

    void pointer()
    {
        line.SetPosition(0, finger_tip.position);

        Ray myRay = new Ray(finger_tip.position, finger_tip.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(myRay, out rayHit, Mathf.Infinity))
        {
            line.SetPosition(1, rayHit.point);
        }
        else
        {
            line.SetPosition(1, finger_tip.position + (finger_tip.forward * 10.0f));
        }

        line.enabled = true;
    }

    void check_teleport()
    {
        Ray myRay = new Ray(finger_tip.position, finger_tip.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(myRay, out rayHit, Mathf.Infinity))
        {
            if (rayHit.collider.gameObject.tag == "Ground")
            {
                player.position = new Vector3(rayHit.point.x, player.position.y, rayHit.point.z);
            }
        }
    }

    bool check_select()
    {
        Ray myRay = new Ray(finger_tip.position, finger_tip.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(myRay, out rayHit, Mathf.Infinity))
        {
            if (rayHit.collider.gameObject.tag == "Selectable")
            {
                //Debug.Log("PC: Object is selectable");

                if (rayHit.collider.gameObject.GetComponent<Item>())
                {
                    Item item = rayHit.collider.gameObject.GetComponent<Item>();
                    if (!selected_items.Contains(item))
                    {
                        selected_items.Add(item);
                        item.select();                   
                    }
                        
                }
                else
                {
                    Debug.Log("PC: Invalid Item");
                }

                /*
                grabbed_object = rayHit.collider.transform;                
                grabbed_vector = rayHit.collider.bounds.center - finger_tip.position; // vector from center of object to finger_tip
                */
                return true;
            }

            // Check if object is a spawner, then have an on click. 

            else if (rayHit.collider.gameObject.tag == "Spawner")
            {
                //Debug.Log("PC: Object is spawner");
                if (rayHit.collider.gameObject.GetComponent<SpawnOfSphere>())
                {
                    rayHit.collider.gameObject.GetComponent<SpawnOfSphere>().OnClick();                    
                }
                else
                {
                    //Debug.Log("PC: Invalid Spawner");
                }
            }
        }
        return false;
    }
}
