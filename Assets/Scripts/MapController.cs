using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MapController : MonoBehaviour, RoomSwitch
{
    public int size;
    // Start is called before the first frame update
    private Room[][] map;
    public Room currentRoom;
    public FieldController field;
    public LimitStack<TimeCapsule> timeStack;
    public GameObject[] eList;
    public GameObject overlay;
    public OptionsUI options;
    public readonly float tcap = 0.2f;
    public float sampleTime;
    private float threshold;
    public int tsSize;
    public TextAsset json;
    private MapGen mp;

    private (int, int) bossRoom;
    void Awake () {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        threshold = tcap;
        mp = new MapGen(5, json);
        GenRooms();

        timeStack = new LimitStack<TimeCapsule>(tsSize);

        InvokeRepeating("AddToTimeStack", sampleTime, sampleTime);
    }

    public void SetStart() {
        bossRoom = (UnityEngine.Random.Range(0, 5), UnityEngine.Random.Range(0, 5));
        //bossRoom = (2,1);
        map[2][2] = new Room("Blank", 2, 2 , new List<Entity>(), new bool[]{true, true, true, true});

        map[bossRoom.Item1][bossRoom.Item2] = new Room("Boss", bossRoom.Item1, bossRoom.Item2, new List<Entity>(), new bool[]{true, true, true, true});

        //Chest cs = new Chest(new Vector3(2,1,2), Quaternion.identity, field.gec.GetModel("Chest"));
        BossEnemy be = new BossEnemy(new Vector3(0,1.38f,0), Quaternion.identity, field.gec.GetModel("BossEnemy"), 100, 0);

        //cs.ForceLootTable(field.gec);
        //map[2][2].Ent.Add(cs);
        map[bossRoom.Item1][bossRoom.Item2].Ent.Add(be);
        //Change later
        currentRoom = map[2][2];
        field.LoadRoom(map[2][2]);
    }

    public void GenRooms() {
        map = mp.GenRooms(field.gec);
        mp.IncDiff();
        SetStart();
    }    
    // Update is called once per frame
    void Update()
    {
        if (options.enable) return;

        if (Keyboard.current.spaceKey.isPressed) {
            Time.timeScale = 0.01f;
            overlay.SetActive(true);
            if (threshold < 0 && field.player.TimeLeft() ) {
                Rewind();
                this.GetComponent<AudioSource>().Play();
                threshold = tcap;
            }
            else {
                threshold -= Time.deltaTime * 100;
            }
        }
        else if (field.player.GetHealth() <= 0 ) {
            Time.timeScale = 0.01f;
            overlay.SetActive(true);
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
        TimeCapsule tc = new TimeCapsule(newRoom, field.player);
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
        SaveRoom();

        currentRoom = past.room;
        field.LoadRoom(past.room);
        field.player.UseTimeCapsule(past);
        
        SaveRoom();
    }
    public void LoadFinished() {
        if (currentRoom.getX() == bossRoom.Item1 && currentRoom.getY() == bossRoom.Item2) {
            Debug.Log("I is at boss");
            options.BossMusic();
        }
        else {
            Debug.Log("I is normal");
            options.NormalMusic();
        }
    }
    public void North() {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (x + 1 == size) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;

        currentRoom = map[x+1][y];
        field.SendMessage("LoadRoom", currentRoom);

    }
    public void South() {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (x - 1 == -1) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;

        currentRoom = map[x-1][y];
        field.SendMessage("LoadRoom", currentRoom);

    }
    public void East () {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (y + 1 == size) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;

        currentRoom = map[x][y+1];
        field.SendMessage("LoadRoom", currentRoom);

    }
    public void West() {
        int x = currentRoom.getX();
        int y = currentRoom.getY();
        if (y - 1 == size) {
            Debug.Log("Uh oh");
        }
        map[x][y] = timeStack.peek().room;

        currentRoom = map[x][y-1];
        field.SendMessage("LoadRoom", currentRoom);
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
        public float hp;
        public BaseWeapon weapon;
        //other player meta here
        public TimeCapsule(Room room, PlayerController player) {
            this.room = room;
            this.pRot = player.transform.rotation;
            this.pPos = player.transform.position;
            this.hp = player.GetHealth();
            weapon = player.weapon.weapon;
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