using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour, RoomSwitch
{
    // Start is called before the first frame update
    int xcoord, ycoord;
    public PlayerController player;
    public MapController map;
    public GameEntitiesController gec;
    private Vector3[] entryPos = {new Vector3(0,1,-4.5f),new Vector3(0,1,4.5f),
         new Vector3 (-4.5f,1,0), new Vector3(4.5f,1,0)};
    private GameObject[] walls;
    private GameObject[] doors;
    private GameObject[] bar;
    private Transform dynamic;
    private Transform stat;
    void Start()
    {
        dynamic = this.transform.GetChild(0);
        stat = this.transform.GetChild(1);
        xcoord = ycoord = -1;
        player.transform.position = new Vector3(0,10,0);

        walls = new GameObject[4];
        for (int i = 0; i < 4; i++) {
            walls[i] = stat.GetChild(i).gameObject;
        }
        doors = new GameObject[4];
        for (int i = 0; i < 4; i++) {
            doors[i] = stat.GetChild(i+4).gameObject;
        }
        bar = new GameObject[4];
        for (int i = 0; i < 4; i++) {
            bar[i] = stat.GetChild(i+8).gameObject;
        }

        InvokeRepeating("CheckForEnemies", 1 , 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckForEnemies () {
        bool val = false;
        for (int i = 0 ; i < dynamic.childCount; i++) {
            if (dynamic.GetChild(i).name.Contains("Enemy")) {
                val = true;
                break;
            }
        }

        foreach (var x in bar) x.SetActive(val);
        
    }
    public Room SaveRoom() {
        List<Entity> allInRoom = new List<Entity>();
        for (int i = 1; i < dynamic.childCount; i++) {
            Transform child = dynamic.GetChild(i);
            Entity temp = child.GetComponent<EntityController>().DeepCopy();
            allInRoom.Add(temp);
        }
        Room room = new Room(map.currentRoom, allInRoom);
        return room;
    }

    public void LoadRoom(Room room) {
        Debug.Log(room);
        xcoord = room.getX();
        ycoord = room.getY();
        bool[] d = room.Doors();
        //load new room

        //remove existing dyn objects
        for (int i = 1; i < dynamic.childCount; i++) {
            Destroy(dynamic.GetChild(i).gameObject);
        }
        
        //Loading walls & doors
        for (int i = 0 ; i < 4; i++) {
            if (d[i]) {
                walls[i].SetActive(false);
                doors[i].SetActive(true);
            }
            else {
                walls[i].SetActive(true);
                doors[i].SetActive(false);
            }
        }
        //Loading Entities
        Debug.Log(room.Ent.Count);
        foreach (Entity x in room.Ent) {
            x.Spawn(this);
        }
        CheckForEnemies();
        map.SendMessage("LoadFinished");
    }

    public Transform GetDynamic () {
        return dynamic;
    }
    public Transform GetStatic () {
        return stat;
    }

    public void North() {
        player.transform.position = entryPos[0];
    }
    public void South() {
        player.transform.position = entryPos[1];
    }
    public void East () {
        player.transform.position = entryPos[2];
    }
    public void West() {
        player.transform.position = entryPos[3];
    }
}
