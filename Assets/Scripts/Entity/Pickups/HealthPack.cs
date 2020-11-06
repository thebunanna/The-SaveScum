using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Pickup
{
    public HealthPack (Vector3 position, Quaternion rotation, GameObject model):
                base(position, rotation, model) {                
    }

    public override void OnCollision(GameObject current, GameObject other) {
        if (other.tag == "Player" && threshold < 0) {
            other.SendMessage("AddHealth", val);
            Debug.Log("boop");
            GameObject.Destroy(current);
        }
    }

    public override Entity DeepCopy (Transform T) {
        var e = new HealthPack(T.position, T.rotation, this.model);
        e.val = this.val;
        return e;
    }
}