using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitController : MonoBehaviour
{
    public string dir;
    private bool open;
    public MapController map;
    // Start is called before the first frame update
    void Start()
    {
        open = true;
        if (dir == null) dir = "n";
    }

    // Update is called once per frame
    void Update()
    {
        //Close exit if enemies are present.
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player") && open)
        {          
            ExecuteEvents.Execute<RoomSwitch>(map.gameObject, null, (x,y)=> {
                switch(dir) {
                    case "n":
                        x.North();
                        break;
                    case "s":
                        x.South();
                        break;
                    case "e":
                        x.East();
                        break;
                    case "w":
                        x.West();
                        break;
                }
            });
            ExecuteEvents.Execute<RoomSwitch>(transform.parent.parent.parent.gameObject, null, (x,y)=> {
                switch(dir) {
                    case "n":
                        x.North();
                        break;
                    case "s":
                        x.South();
                        break;
                    case "e":
                        x.East();
                        break;
                    case "w":
                        x.West();
                        break;
                }
            });
        }
    }
}
