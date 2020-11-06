using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUIController : MonoBehaviour
{
    public WeaponController weapon;
    public Sprite[] images;
    private Text view;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {   
        view = this.transform.GetChild(0).GetComponent<Text>();
        image = this.transform.GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        view.text = $"{weapon.weapon.durability}";
        if (weapon.weapon.name == "Pistol") image.sprite = images[0];
        else if (weapon.weapon.name == "Rifle") image.sprite = images[1];
        else if (weapon.weapon.name == "Shotgun") image.sprite = images[2];
        else Debug.Log("wtf");
    }
}
