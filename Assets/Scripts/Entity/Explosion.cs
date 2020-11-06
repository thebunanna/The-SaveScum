using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Entity {
    float timer;
    public Explosion (Vector3 position, Quaternion rotation, GameObject model, float time) {
        SetVals(position, rotation, model);
        this.timer = time;
    }

    public override void Update(GameObject current) {
        if (timer < 0) {
            current.SendMessage("Remove");
        }
        else {
            timer -= Time.deltaTime;
        }
    }

    public override Entity DeepCopy(Transform T) {
        return new Explosion (this.position, this.rotation, this.model, timer);
    }

}