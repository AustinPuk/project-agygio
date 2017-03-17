using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour {

    [SerializeField]
    string file_name = "test.txt";

    [SerializeField]
    MiniManager mini_manager;

    [SerializeField]
    bool save_file;

    [SerializeField]
    bool load_file;

    [SerializeField]
    GameObject[] items;

    private StreamWriter file;

    // Use this for initialization
    void Start () {       
        save_file = false;
        load_file = false;
	}

    void Update()
    {
        if (save_file)
        {            
            RecordData();
            save_file = false;
        }

        if (load_file)
        {
            LoadData();
            load_file = false;
        }
    }
	
	public void RecordData()
    {
        file = new StreamWriter(file_name);

        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            //Saves Object Type, Position, and Rotation
            GameObject obj = this.gameObject.transform.GetChild(i).gameObject;
            if (obj.GetComponent<Item>() && obj.activeSelf)
            {
                Item item = obj.GetComponent<Item>();
                string name = item.object_type;
                Vector3 pos = obj.transform.position;
                Vector3 rot = obj.transform.rotation.eulerAngles;
                file.WriteLine(name + " | " + pos.x + " | " + pos.y + " | " + pos.z + " | " +
                               rot.x + " | " + rot.y + " | " + rot.z);                
            }
        }

        Debug.Log("File Save Complete");
        file.Close();
    }

    public void LoadData()
    {
        Debug.Log("Loading New Room");
        //Parse file and create new room
        string[] lines = File.ReadAllLines(file_name);

        if (lines.GetLength(0) <= 0)
        {
            Debug.Log("Unloadable File Detected, Loading Cancelled");
            return;
        }

        Debug.Log("Deleting Old Old");
        //Delete current room
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); //lol
        }
        

        foreach(string line in lines)
        {
            string[] tokens = line.Split();
            string obj = tokens[0];
            Vector3 pos = new Vector3(float.Parse(tokens[2]), float.Parse(tokens[4]), float.Parse(tokens[6]));
            Vector3 rot = new Vector3(float.Parse(tokens[8]), float.Parse(tokens[10]), float.Parse(tokens[12]));

            GameObject item = null;

            if (obj == "TV")
                item = Instantiate(items[0]);
            else if (obj == "CABINET")
                item = Instantiate(items[1]);
            else if (obj == "CHAIR")
                item = Instantiate(items[2]);
            else if (obj == "DESK1")
                item = Instantiate(items[3]);
            else if (obj == "DESK2")
                item = Instantiate(items[4]);
            else if (obj == "LOCKER")
                item = Instantiate(items[5]);
            else if (obj == "WHITEBOARD")
                item = Instantiate(items[6]);

            item.transform.position = pos;
            item.transform.rotation = Quaternion.Euler(rot);
            item.transform.parent = this.transform;
        }

        Debug.Log("File Load Complete");


        Debug.Log("Recreating Miniature");
        mini_manager.build_minature();
    }
}
