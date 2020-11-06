using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet {
    Vector3 force;
    GameObject source;
    public EnemyBullet (Vector3 position, Quaternion rotation, GameObject model, 
            Vector3 force, GameObject source) {
        SetVals(position, rotation, model);
        this.force = force;
        this.source = source;
        
    }

    public override Entity DeepCopy(Transform T) {
        return new EnemyBullet (T.position, T.rotation, this.model, this.force, this.source);
        //finish this plz
    }

    public override void OnCollision (GameObject current, GameObject other) {
        base.OnCollision(current, other);
        if (other.tag == "Player") {
            current.SendMessage("Remove");
            other.SendMessage("TakeDamage", 2);
        }  
    }

    public override GameObject Spawn(FieldController field) {
        var c = base.Spawn(field);
        var rb = c.GetComponent<Rigidbody>();
        rb.AddForce(force);
        Debug.Log($"Force:{force}, vel: {rb.velocity}");
        return c;
    }
}