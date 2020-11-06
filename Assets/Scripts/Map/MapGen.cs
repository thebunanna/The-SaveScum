using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapGen 
{
    private int size;
    private int rsize;
    private Dictionary<string, List<char[][]>> ld;
    private (string, float)[] types = new (string, float)[] {("Empty", 0.10f), ("Enemy", 0.65f), ("Trap", 0.15f), ("Reward", 0.1f)};
    private int diff;
    public MapGen () {
        size = 5;
        rsize = 10;
        diff = 1;
    }
    public MapGen (int size, TextAsset json) {
        this.size = size;
        rsize = 10;
        LayoutList ll = JsonUtility.FromJson<LayoutList>(json.text);
        ld = Decrypt(ll);
        diff = 1;
    }

    public Room[][] GenRooms(GameEntitiesController gec) {

        Room[][] map = new Room[size][];
        for (int i = 0; i < size; i++) {
            map[i] = new Room[size];
            for (int j = 0; j < size; j++) {
                map[i][j] = GenerateRandomRoom(i,j, gec);
                
            }
        }
        return map;
    }
    
    private Dictionary<string, List<char[][]>> Decrypt (LayoutList ll) {
        Dictionary<string, List<char[][]>> dicted = new Dictionary<string, List<char[][]>>();
        foreach (var x in this.types) {
            dicted[x.Item1] = new List<char[][]>();
        }
        foreach (var x in ll.layouts) {
            char[][] mm = new char[x.size][];
            for (int i = 0; i < x.size; i++) {
                mm[i] = x.map[i].ToCharArray();
            }
            dicted[x.type].Add(mm);
        }
        return dicted;
    }

    public void Sample() {
        LayoutList ll = new LayoutList();
        ll.layouts = new Layout[1];
        ll.layouts[0] = new Layout();
        ll.layouts[0].type = "sample";
        ll.layouts[0].size = rsize;    
        ll.layouts[0].map = new string[rsize];
        for (int i = 0; i < rsize; i++) {
            for (int j = 0; j < rsize; j++) ll.layouts[0].map[i] += "S";
        }
        Debug.Log (JsonUtility.ToJson(ll));
    }

    private Room GenerateRandomRoom(int x, int y, GameEntitiesController gec) {
        string type = GenerateType();
        bool[] isOpen = new bool[4];
        if (x + 1 != size) isOpen[0] = true;
        if (x - 1 != -1) isOpen[1] = true;
        if (y + 1 != size) isOpen[2] = true;
        if (y - 1 != -1) isOpen[3] = true;

        var dict = ld[type][Random.Range(0, ld[type].Count)];
        var elist = new List<Entity>();
        if (type == "Enemy") {
            Debug.Log("EN");
            elist = CreateEnemyRoom(dict, gec, diff);
        }
        else if (type == "Empty") {
            Debug.Log("E");
            elist = CreateEmptyRoom(dict, gec);
        }
        else if (type == "Trap") {
            Debug.Log("T");
            elist = CreateTrapRoom(dict, gec, diff);
        }
        else if (type == "Reward") {
            Debug.Log("R");
            elist = CreateRewardRoom(dict, gec, diff);
        }

        return new Room(type, x, y, elist, isOpen);
    }

    private string GenerateType () {
        float rnum = Random.Range(0,1f);
        float runningSum = 0;
        
        for (int i = 0; i < types.Length; i++){
            runningSum += types[i].Item2;
            if (rnum <= runningSum) {
                return types[i].Item1;
            }
        }

        return types[types.Length].Item1;
    }

    private List<Entity> CreateEmptyRoom (char[][] dict, GameEntitiesController gec) {
        var elist = new List<Entity>();

        for (int i = 0; i < rsize; i++) {
            for (int j = 0; j < rsize; j++) {
                Entity created = default(Entity);
                char c = dict[i][j];
                string eType = "";

                Vector3 pos = new Vector3(i-4.5f, 0.1f, j-4.5f);
                switch (c) {

                    case 'R':
                        if (Random.Range(0, 1f) > 0.7f) {
                            goto case 'C';
                        }
                        else {
                            goto case 'W';
                        }
                    case 'E':
                        goto case 'W';                                                
                    case 'W':
                        eType = "WallHigh";
                        created = new Entity(pos, Quaternion.identity, gec.GetModel(eType));
                        break;
                    case 'T':
                        eType = "Trap";
                        break;
                    case 'C':
                        eType = "Chest";
                        Chest ec = new Chest(pos + new Vector3(0,1,0), Quaternion.identity, gec.GetModel(eType));
                        ec.ForceLootTable(gec);
                        created = ec;
                        break;
                    default:
                        eType = "";
                        break;
                }
                if (eType != "") elist.Add(created);                

            }
        }
        return elist;
    }
    private List<Entity> CreateEnemyRoom(char[][] dict, GameEntitiesController gec, int difficulty) {

        var eCount = 2;
        var elist = new List<Entity>();

        for (int i = 0; i < rsize; i++) {
            for (int j = 0; j < rsize; j++) {
                Entity created = default(Entity);
                char c = dict[i][j];
                string eType = "";

                Vector3 pos = new Vector3(i-4.5f, 0.1f, j-4.5f);
                switch (c) {
                    case ' ':
                        if (Random.Range(0, 1f) < ((difficulty - 1) * 0.03)) {
                            goto case 'E';
                        }
                        eType = "";
                        break;
                    case 'R':
                        if (Random.Range(0, 1f) > 0.5) {
                            goto case 'E';
                        }
                        else {
                            goto case 'T';
                        }
                    case 'E':
                        //add more enemy types
                        eType = "Enemy" + Random.Range(0, eCount);
                        if (eType == "Enemy0") {
                            created = new GenericEnemy(pos + new Vector3(0,1,0), 
                                Quaternion.identity, gec.GetModel(eType), 5 * difficulty, 2);
                        }
                        else if (eType == "Enemy1") {
                            created = new SuicideEnemy(pos + new Vector3(0,1,0), 
                                Quaternion.identity, gec.GetModel(eType), 5 * difficulty, 2);
                        }
                        
                        break;
                    case 'W':
                        eType = "WallHigh";
                        created = new Entity(pos, Quaternion.identity, gec.GetModel(eType));
                        break;
                    case 'T':
                        eType = "Trap";
                        GenericTrap trap = new GenericTrap(pos - new Vector3(0, 0.2f, 0), Quaternion.identity, gec.GetModel("Trap"));
                        created = trap;
                        break;
                    default:
                        eType = "";
                        break;
                }
                if (eType != "") elist.Add(created);                

            }
        }
        return elist;
    }

    private List<Entity> CreateTrapRoom (char[][] dict, GameEntitiesController gec, int difficulty) {
        var elist = new List<Entity>();

        for (int i = 0; i < rsize; i++) {
            for (int j = 0; j < rsize; j++) {
                Entity created = default(Entity);
                char c = dict[i][j];
                string eType = "";

                Vector3 pos = new Vector3(i-4.5f, -0.2f, j-4.5f);
                switch (c) {
                    case ' ':
                        if (Random.Range(0, 1f) < ((difficulty - 1) * 0.03)) {
                            goto case 'T';
                        }
                        eType = "";
                        break;
                    case 'T':
                        //add more enemy types
                        eType = "Trap";
                        GenericTrap trap = new GenericTrap(pos, Quaternion.identity, gec.GetModel("Trap"));
                        created = trap;
                        break;
                    case 'W':
                        eType = "WallHigh";
                        created = new Entity(pos, Quaternion.identity, gec.GetModel(eType));
                        break;
                    default:
                        goto case 'T';                        
                }
                if (eType != "") elist.Add(created);                

            }
        }
        return elist;
    }

    private List<Entity> CreateRewardRoom (char[][] dict, GameEntitiesController gec, int difficulty) {
        var elist = new List<Entity>();

        for (int i = 0; i < rsize; i++) {
            for (int j = 0; j < rsize; j++) {
                Entity created = default(Entity);
                char c = dict[i][j];
                string eType = "";

                Vector3 pos = new Vector3(i-4.5f, -0.2f, j-4.5f);
                switch (c) {
                    case ' ':
                        eType = "";
                        break;
                    case 'T':
                        //add more enemy types
                        eType = "Trap";
                        GenericTrap trap = new GenericTrap(pos, Quaternion.identity, gec.GetModel("Trap"));
                        created = trap;
                        break;
                    case 'W':
                        eType = "WallHigh";
                        created = new Entity(pos, Quaternion.identity, gec.GetModel(eType));
                        break;
                    case 'C':
                        eType = "Chest";
                        Chest ec = new Chest(pos + new Vector3(0,1,0), Quaternion.identity, gec.GetModel(eType));
                        ec.ForceLootTable(gec);
                        created = ec;
                        if (Random.Range(0, 1f) < ((difficulty - 1) * 0.03)) {
                            elist.Add(new GenericTrap(pos, Quaternion.identity, gec.GetModel("Trap")));
                        }
                        break;
                    default:
                        goto case 'T';                        
                }
                if (eType != "") elist.Add(created);                

            }
        }
        return elist;
    }
    public void IncDiff () {
        diff += 1;
    }
}