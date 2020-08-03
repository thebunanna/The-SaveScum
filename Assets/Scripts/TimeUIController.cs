using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUIController : MonoBehaviour
{
    public PlayerController player;
    private Text view;
    // Start is called before the first frame update
    void Start()
    {   
        view = this.transform.GetChild(0).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        view.text = $"Time: {(int)player.GetCurrentTime()} / {(int)player.GetMaxTime()}";
    }
}
