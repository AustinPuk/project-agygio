using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Item : MonoBehaviour {

    [SerializeField]
    public string object_type;

    [SerializeField]
    GameObject highlight;

    [SerializeField]
    ItemCollider my_collider;

    public bool selected;
    public bool within_bounds;    

    public Vector3 grab_vector;
    public Vector3 last_grab;
    public Vector3 last_loc;
    public Vector3 last_rot;

    public bool constrain_x; //For the dang whiteboards
    public bool constrain_z;
    public bool constrain_rot;

    void Start()
    {        
        selected = false;
        within_bounds = false;
        grab_vector = new Vector3(0.0f, 0.0f, 0.0f);
        last_grab = grab_vector;
        last_loc = this.transform.position; //Last Iteration
        highlight.SetActive(false);
    }        

    public void move(Transform finger_tip, float dist_offset, Vector3 euler_rot)
    {
        float local_x = transform.localPosition.x;
        float local_y = transform.localPosition.y;
        float local_z = transform.localPosition.z;

        //Moves by first moving its collider, then if the collider is in a valid position, move the actual object.
        if (my_collider.collisions == 0)
        {
            this.transform.position = last_loc;
            if (constrain_x)
                this.transform.localPosition = new Vector3(local_x, local_y, this.transform.localPosition.z);
            else if (constrain_z)
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, local_y, local_z);
            else
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, local_y, this.transform.localPosition.z); //Keeps it on the ground
            if (!constrain_rot)
                this.transform.Rotate(euler_rot);
            grab_vector = last_grab;
        }        

        //Setting Grab Vector for Moving with Finger Tip
        if (grab_vector == new Vector3(0.0f, 0.0f, 0.0f))
            set_vector(finger_tip);

        Vector3 new_grab = grab_vector + (Vector3.Normalize(grab_vector) * dist_offset);

        Vector3 new_loc = finger_tip.position + (finger_tip.rotation * new_grab);

        if (constrain_x)
        {
            new_loc = new Vector3(this.transform.position.x, this.transform.position.y, new_loc.z);
        }
        else if (constrain_z)
        {
            new_loc = new Vector3(new_loc.x, this.transform.position.y, this.transform.position.z);
        }
        else
            new_loc = new Vector3(new_loc.x, this.transform.position.y, new_loc.z);

        my_collider.transform.position = new_loc;

        if (!constrain_rot)
        {
            my_collider.transform.rotation = this.transform.rotation;
            my_collider.transform.Rotate(euler_rot);
        }        

        last_loc = new_loc;
        last_rot = euler_rot;
        last_grab = new_grab;
    }

    public void set_vector(Transform finger_tip)
    {
        grab_vector = Quaternion.Inverse(finger_tip.rotation) * (this.GetComponent<BoxCollider>().bounds.center - finger_tip.position);
        last_grab = grab_vector;
    }

    public void select()
    {        
        selected = true;
        highlight.SetActive(true);
        last_loc = this.transform.position;
    }

    public void deselect()
    {
        //this.transform.position = last_valid_loc; //Saves to last valid location
        grab_vector = new Vector3(0.0f, 0.0f, 0.0f);
        last_grab = grab_vector;
        selected = false;
        highlight.SetActive(false);
        my_collider.transform.position = this.transform.position;
        my_collider.transform.rotation = this.transform.rotation;
    }
}
