using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : Bullet {
    Vector3 force;
    GameObject source;
    GameObject target;
    public EnemyMissile (Vector3 position, Quaternion rotation, GameObject model, 
            Vector3 force, GameObject source, GameObject target) {
        SetVals(position, rotation, model);
        this.force = force;
        this.source = source;
        this.target = target;
        
    }

    public override void Update(GameObject current) {
        base.Update(current);

        if (target == null) SetTarget(field.player.gameObject);
        
        RaycastHit hit;
        Vector3 ctp = current.transform.position;
        Vector3 ttp = target.transform.position;
        if (Physics.Raycast (ctp, (ttp - ctp).normalized, out hit, 10)) {

            if (hit.transform.tag == "Player") {
                 Vector3 dir = ttp - ctp;
                dir = dir.normalized;
                var rb = current.GetComponent<Rigidbody>();
                rb.AddForce(dir * 10);
                if (rb.velocity.magnitude > 20) {
                    rb.velocity = rb.velocity.normalized * 20;
                }

        
                //create the rotation we need to be in to look at the target
                Quaternion _lookRotation = Quaternion.LookRotation(dir);
        
                //rotate us over time according to speed until we are in the required rotation
                current.transform.rotation = Quaternion.Slerp(current.transform.rotation, _lookRotation, Time.deltaTime * 10);
            }
           
        //there is something in the way
        }

    }

    public void SetTarget(GameObject target) {
        this.target = target;
    }
    public GameObject GetTarget() {
        return this.target;
    }
    public override Entity DeepCopy(Transform T) {
        return new EnemyMissile(T.position, T.rotation, this.model, 
                T.GetComponent<Rigidbody>().velocity, this.source, this.target);
    }

    public override void OnCollision(GameObject current, GameObject other) {        
        if (other.tag == "Player") {
            other.SendMessage("TakeDamage", 1);
        }
        ExplosionDamage(current);
        current.SendMessage("Remove");
    }

    public void ExplosionDamage (GameObject current) {
        Collider[] hc = Physics.OverlapSphere(current.transform.position, 1.5f);
        foreach (var x in hc) {
            if (x.tag == "Player") x.SendMessage("TakeDamage", 1.5f);
            var rb = x.GetComponent<Rigidbody>();
            if (rb != null) rb.AddExplosionForce(1500, current.transform.position, 2, 0.0F);
        }
        var e = new Explosion (current.transform.position + new Vector3 (0, 2, 0), Quaternion.identity, field.gec.GetModel("TinyExplosion"), 2);
        var g = e.Spawn(field);
        g.GetComponent<ParticleSystem>().Play();
    }
}