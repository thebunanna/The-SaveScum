using UnityEngine;
using System.Collections.Generic;
using System;

public class Shotgun : BaseWeapon {

    protected int pellets;
    
    public Shotgun () {
        delay = 1.5f;
        dmg = 1.5f;
        durability = 30;
        name = "Shotgun";
        ammo = "Bullet1";
        deviation = 25;
        speed = 40;
        pellets = 5;
    }

    public override List<(float, string)> Fire() {
        durability -= 1;
        List<(float,string)> projectiles = new List<(float, string)>();
        for (int i = 0 ; i< pellets; i++) {
            projectiles.Add((UnityEngine.Random.Range(-1 * deviation, deviation) , ammo));
        }
        return projectiles;
    }

    public override void Upgrade() {
        level ++;
        durability += level * 15;
        pellets += level % 3 == 0 ? 1 : 0;
        delay -= level % 2 == 0 ? 0.05f : 0;
        deviation -= level % 4 == 0 ? 2 : 0;
        dmg += 0.1f * level;
    }
}