using UnityEngine;
using System.Collections.Generic;

public class BasicPistol : BaseWeapon {
    public BasicPistol () {
        delay = 0.3f;
        dmg = 0.6f;
        durability = -1;
        name = "Pistol";
        ammo = "Bullet1";
        deviation = 2;
        speed = 80;
    }

}