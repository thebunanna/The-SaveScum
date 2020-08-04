using UnityEngine;
using System.Collections.Generic;

public class BasicPistol : BaseWeapon {
    public BasicPistol () {
        delay = 0.3f;
        dmg = 1;
        durability = -1;
        name = "Pistol";
        ammo = "basic";
        deviation = 2;
        speed = 800;
    }

}