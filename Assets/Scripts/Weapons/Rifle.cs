using UnityEngine;
using System.Collections.Generic;
using System;

public class Rifle : BaseWeapon {

    protected int pellets;
    
    public Rifle () {
        delay = 0.15f;
        dmg = 0.8f;
        durability = 300;
        name = "Rifle";
        ammo = "Bullet1";
        deviation = 5;
        speed = 60;
        pellets = 1;
    }

    public override List<(float, string)> Fire() {
        durability -= 1;
        List<(float,string)> projectiles = new List<(float, string)>();
        for (int i = 0 ; i < pellets; i++) {
            projectiles.Add((UnityEngine.Random.Range(-1 * deviation, deviation) , ammo));
        }
        return projectiles;
    }

    public override void Upgrade() {
        level ++;
        durability += level * 150;
        delay -= level % 2 == 0 ? 0.02f : 0;
        deviation -= level % 3 == 0 ? 1 : 0;
        dmg += 0.2f * level;
    }
}