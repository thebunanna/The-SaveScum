using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : Pickup
{
    public NextLevel (Vector3 position, Quaternion rotation, GameObject model):
                base(position, rotation, model) 
    {             
        this.val = Random.Range(0, 2);       
    }

    public override void OnCollision(GameObject current, GameObject other) {
        if (other.tag == "Player" && threshold < 0) {
            other.SendMessage("NextLevel", (int)val);
            GameObject.Destroy(current);
        }
    }
    public override Entity DeepCopy (Transform T) {
        var e = new NextLevel(T.position, T.rotation, this.model);
        e.val = this.val;
        return e;
    }
}