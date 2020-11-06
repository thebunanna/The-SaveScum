using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Entity {
    float hp;
    List<Entity> loot;
    public Chest(Vector3 position, Quaternion rotation, GameObject model) {
        SetVals(position, rotation, model);
        hp = 5;
    }
 
    public override void Update(GameObject current) {
        if (loot == null && field.gec != null) {
            loot = new List<Entity>();
            loot.Add(new HealthPack(position, rotation, field.gec.GetModel("Heal")));
            loot.Add(new TimePack(position, rotation, field.gec.GetModel("Time")));
        }
    }
    public override void OnCollision (GameObject current, GameObject other) {
        if (other.tag == "Player_Proj") {
            Bullet b = (Bullet) other.GetComponent<EntityController>().GetMeta();
            hp -= b.GetSource().GetComponent<WeaponController>().weapon.dmg;

            GameObject.Destroy(other);

            if (hp < 0) GameObject.Destroy(current);
        }
        if (other.tag == "Player") {
            foreach (var x in loot) {
                // if (Random.Range(0, 1.0f) > x.Item2) {
                //     var e = x.Item1;
                    
                // }
                x.Spawn(field);
            }
            GameObject.Destroy(current);

        }
    } 

    public void ForceLootTable (GameEntitiesController gec) {
        loot = new List<Entity>();
        if (GenRand(0.7f)) {
            loot.Add(new HealthPack(position, rotation, gec.GetModel("Heal")));
        }
        if (GenRand(0.7f)) {
            loot.Add(new TimePack(position, rotation, gec.GetModel("Time")));
        }
        if (GenRand(0.7f)) {
            loot.Add(new GunPack(position, rotation, gec.GetModel("GPack")));
        }
    }

    public override Entity DeepCopy(Transform T) {
        var x = new Chest (position, rotation, model);
        x.loot = new List<Entity>();
        foreach (var e in this.loot) x.loot.Add(e);
        return x;
    }
}