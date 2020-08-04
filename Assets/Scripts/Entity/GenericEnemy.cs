using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : Entity {
    protected float hp;
    protected List<string> drops;
    public GenericEnemy (Vector3 position, Quaternion rotation, GameObject model) {
        base.SetVals(position,rotation,model);
        hp = 10;
        drops = null;
    }

    public override void OnCollision(GameObject current, GameObject other) {
        if (other.tag == "Player_Proj") {
            Bullet b = (Bullet) other.GetComponent<EntityController>().GetMeta();
            
            hp -= b.GetSource().GetComponent<WeaponController>().weapon.dmg;
            other.SendMessage("Remove");
        }
        if (hp < 0) current.SendMessage("Remove");
    }
}