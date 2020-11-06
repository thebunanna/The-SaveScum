using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Entity
{
    protected float val;
    protected float threshold = 1;
    public Pickup (Vector3 position, Quaternion rotation, GameObject model) {
        SetVals(position,rotation,model);
        val = Random.Range(1.0f, 5.0f);
    }

    public override void Update(GameObject current) {
        threshold -= Time.deltaTime;
    }

    public override Entity DeepCopy (Transform T) {
        var e = new Pickup(T.position, T.rotation, this.model);
        e.val = this.val;
        return e;
    }
}