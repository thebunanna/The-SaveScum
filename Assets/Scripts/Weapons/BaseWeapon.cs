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
    public int level = 1;
    public BaseWeapon () {
        delay = 0.1f;
        dmg = 0;
        durability = -1;
        name = "base";
        ammo = "Bullet1";
        deviation = 0;
        speed = 100;
    }

    public virtual List<(float, string)> Fire() {
        List<(float,string)> projectiles = new List<(float, string)>();
        var p1 = (UnityEngine.Random.Range(-1 * deviation, deviation) , ammo);
        projectiles.Add(p1);
        return projectiles;
    }

    public virtual void Upgrade () {
        dmg += 1;        
        level ++;
        return;
    }

    public virtual void DeepCopy() {
        
    }
}