using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Entity {
    Vector3 force;
    GameObject source;
    public Bullet() {}
    public Bullet (Vector3 position, Quaternion rotation, GameObject model, Vector3 force, GameObject source) {
        SetVals(position, rotation, model);
        this.force = force;
        this.source = source;
    }

    public override void Update(GameObject current) {
        Vector3 cpos = current.transform.position;
        if (Mathf.Abs(cpos.x) > 10 || Mathf.Abs(cpos.z) > 10) current.SendMessage("Remove");
    }
    public override void OnCollision (GameObject current, GameObject other) {
        if (other.tag == "Wall" || other.tag == "Doors") {
            current.SendMessage("Remove");
        }  
    }

    public override Entity DeepCopy(Transform T) {
        return new Bullet (T.position, T.rotation, this.model, this.force, this.source);
        //finish this plz
    }

    public override GameObject Spawn(FieldController field) {
        GameObject c = base.Spawn(field);
        c.GetComponent<Rigidbody>().AddForce(force);
        return c;
    }

    public GameObject GetSource() {
        return source;
    }
    public void SetSource(GameObject source) {
        this.source = source;
    }
    
}