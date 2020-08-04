using UnityEngine;
using System.Collections.Generic;
using System;
public class BaseWeapon {
    public float delay;
    public float dmg;
    public int durability;
    public string name;
    public float deviation;
    public string ammo;
    public float speed;
    public BaseWeapon () {
        delay = 0.1f;
        dmg = 0;
        durability = -1;
        name = "base";
        ammo = "null";
        deviation = 0;
        speed = 1000;
    }

    public virtual List<Tuple<float, string>> Fire() {
        List<Tuple<float, string>> projectiles = new List<Tuple<float, string>>();
        var p1 = new Tuple<float, string> (UnityEngine.Random.Range(-1 * deviation, deviation) , ammo);
        projectiles.Add(p1);
        return projectiles;
    }
}