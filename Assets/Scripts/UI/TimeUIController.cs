using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUIController : MonoBehaviour
{
    public PlayerController player;
    private Image view;
    // Start is called before the first frame update
    void Start()
    {   
        view = this.transform.GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        view.fillAmount = player.GetCurrentTime() / player.GetMaxTime();
    }
}
