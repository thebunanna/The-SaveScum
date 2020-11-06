using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTrap : Entity {

    public GenericTrap (Vector3 position, Quaternion rotation, GameObject model) {
        SetVals(position,rotation,model);
        triggered = false;
        state = 100;
    }
    protected bool triggered;
    protected float state;

    public override void Update(GameObject current) {
        if (triggered && state > 0) {
            foreach (Transform child in current.transform) {
                var r = child.GetComponent<Renderer>();
                if (r != null) {
                    r.material.color = Color.Lerp (r.material.color, Color.red, Time.deltaTime / 3);
                }
            }
            state -= Time.deltaTime;
        }
        if (state < 0) {
            Collider[] hc = Physics.OverlapSphere(current.transform.position, 4);
            foreach (var x in hc) {
                if (x.tag == "Player") x.SendMessage("TakeDamage", 2);
                var rb = x.GetComponent<Rigidbody>();
                if (rb != null) rb.AddExplosionForce(1000, current.transform.position, 4, 1.0F);
            }
            current.SendMessage("Remove");
            var e = new Explosion (current.transform.position + new Vector3 (0, 2, 0), Quaternion.identity, field.gec.GetModel("TinyExplosion"), 0.5f);
            var g = e.Spawn(field);
            g.GetComponent<ParticleSystem>().Play();
        }
    }
    public override void OnCollision(GameObject current, GameObject other) {
        if (other.tag == "Player") {
            triggered = true;
            state = 3f;
        }
    }
    public override void OnTrigger(GameObject current, GameObject other) {
        if (other.tag == "Player") {
            triggered = true;
            state = 3f;
        }
    }

    public override Entity DeepCopy(Transform T) {
        var e = new GenericTrap(position, rotation, model);
        e.triggered = this.triggered;
        e.state = this.state;
        return e;
    }
}