using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapController : MonoBehaviour, RoomSwitch
{
    public int size;
    // Start is called before the first frame update
    private Room[][] map;
    public Room currentRoom;
    public FieldController field;
    public LimitStack<TimeCapsule> timeStack;
    public GameObject entityHolder;
    public GameObject[] eList;
    public GameObject overlay;
    public readonly float tcap = 1;
    private float sampleTime;
    public float threshold;
    void Start()
    {
        sampleTime = 0.5f;
        threshold = tcap;
        //Application.targetFrameRate = 60;

        map = new Room[size][];
        for (int i = 0; i < size; i++) {
            map[i] = new Room[size];
            for (int j = 0; j < size; j++) {
                bool[] isOpen = new bool[4];
                if (i + 1 != size) isOpen[0] = true;
                if (i - 1 != -1) isOpen[1] = true;
                if (j + 1 != size) isOpen[2] = true;
                if (j - 1 != -1) isOpen[3] = true;
                
                map[i][j] = new Room("Basic", i, j, new List<Entity>(), isOpen);
            }
        }

        overlay.SetActive(false);

        timeStack = new LimitStack<TimeCapsule>();

        Entity sample = new Entity();
        sample.SetVals(new Vector3(2,0.5f,2), Quaternion.identity, eList[0]);
        //Change later
        currentRoom = map[2][2];
        map[2][2].Ent.Add(sample);

        InvokeRepeating("AddToTimeStack", sampleTime, sampleTime);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && field.player.TimeLeft()) {
            Time.timeScale = 0.01f;
            overlay.SetActive(true);
            if (threshold < 0) {
                Rewind();
                threshold = tcap;
            }
            else {
                threshold -= Time.deltaTime * 100;
            }
        }
        else {
            threshold = tcap;
            Time.timeScale = 1f;
            overlay.SetActive(false);
        }
    }

    void AddToTimeStack () {
        //When in time stop, don't bother adding to time stack
        if (overlay.activeSelf) return;
        //Save Room every 1 second for time travel
        Room newRoom = this.SaveRoom();
        TimeCapsule tc = new TimeCapsule(newRoom, field.player.transform);
        timeStack.push(tc);
    }

    public Room SaveRoom() {

        //Get everything in field dynamic and save to map
        currentRoom = field.SaveRoom();
        map[currentRoom.getX()][currentRoom.getY()] = currentRoom;
        return currentRoom;
    }

    public void Rewind () {
        //Main function and gimmick here
        if (!field.player.UseTime()) {
            return;
        }

        TimeCapsule past = timeStack.pop();
        if (past.Equals(default(TimeCapsule))) {
            Debug.Log("No more time left");
            field.player.RefundTime();
            return;
        }
        
        field.LoadRoom(past.room);
        field.player.UseTimeCapsule(past);
        SaveRoom();
    }
    public void North() {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (x + 1 == size) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;
        currentRoom = map[x+1][y];
    }
    public void South() {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (x - 1 == -1) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;
        currentRoom = map[x-1][y];
    }
    public void East () {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (y + 1 == size) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;
        currentRoom = map[x][y+1];
    }
    public void West() {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (y - 1 == size) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;
        currentRoom = map[x][y-1];
    }

    public struct Room {
        string T;
        int X,Y;
        public List<Entity> Ent;
        bool[] doors;
        public Room (string type, int x, int y, List<Entity> Entities, bool[] isOpen) {
            T = type;
            X = x;
            Y = y;
            Ent = Entities;
            doors = isOpen;
        }
        public Room (Room before, List<Entity> entities) {
            T = before.T;
            X = before.X;
            Y = before.Y;
            Ent = entities;
            doors = before.doors;
        }
        public int getX () {return X;}
        public int getY () {return Y;}
        public bool N() {return doors[0];}
        public bool S() {return doors[1];}
        public bool E() {return doors[2];}
        public bool W() {return doors[3];}

        public bool[] Doors() {return doors;}

        public override string ToString() => $"{X}, {Y}";
    }
    public class LimitStack<T> {
        private int cap;
        private T[] inner;
        private int index = 0;
        public LimitStack() {
            cap = 10;
            inner = new T[cap];
        }
        public LimitStack(int cap) {
            this.cap = cap;
            inner = new T[cap];
        }

        public void push (T item) {
            if (index == cap) index = 0;
            inner[index] = item;
            index ++;
        }
        public T pop () {
            index--;
            if (index == -1) index = cap - 1;
            T temp = inner[index];
            inner[index] = default(T);
            return temp;
        }

        public T peek () {
            return inner[index-1];
        }
    }
    public struct TimeCapsule {
        public Room room;
        public Quaternion pRot;
        public Vector3 pPos;

        //other player meta here
        public TimeCapsule(Room room, Transform playerTransform) {
            this.room = room;
            this.pRot = playerTransform.rotation;
            this.pPos = playerTransform.position;
        }
        
    }
}

public interface RoomSwitch : IEventSystemHandler 
{
    void North();
    void South();
    void East();
    void West();
}