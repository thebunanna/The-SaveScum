using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class SuicideEnemy : GenericEnemy {
    protected bool triggered;
    protected float state;
    protected Vector3 nextPos;
    public SuicideEnemy (Vector3 position, Quaternion rotation, 
        GameObject model, float hp, float speed) : base (position,rotation, model, hp, speed)
    { 
        triggered = false;
        hp /= 2;
        state = 100;
        nextPos = GetRandomPos();
    }

    public override void Update(GameObject current) {
        base.Update(current);

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
                if (rb != null) rb.AddExplosionForce(4000, current.transform.position, 4, 1.0F);
            }
            current.SendMessage("Remove");
            var e = new Explosion (current.transform.position + new Vector3 (0, 2, 0), Quaternion.identity, field.gec.GetModel("TinyExplosion"), 2);
            var g = e.Spawn(field);
            g.GetComponent<ParticleSystem>().Play();
        }
    }

    public override Entity DeepCopy(Transform T) {
        List<Entity> copydrops = new List<Entity>();
        foreach (var x in drops) copydrops.Add(x);

        (string, float)[] copyactions = new (string, float)[actions.Length];
        for (int i = 0; i < actions.Length; i++) {
            copyactions[i] = (actions[i].Item1, actions[i].Item2);
        }

        var en = new SuicideEnemy(position, rotation, model, hp, speed);
        en.SetDrops(copydrops);
        en.SetActions(copyactions);
        en.triggered = this.triggered;
        en.state = this.state;
        en.nextPos = new Vector3(nextPos.x,nextPos.y,nextPos.z);
        return en;
    }
    public override void OnCollision(GameObject current, GameObject other) {
        base.OnCollision(current, other);
        if (other.tag == "Player") {
            triggered = true;
            state = 1;
            actions[index].Item2 = 10;
        }
    }
    public override void OnTrigger(GameObject current, GameObject other) {
        if (other.tag == "Player") {
            triggered = true;
            state = 1;
            actions[index].Item2 = 10;
        }
    }
    public override void SetDefault() {
        drops = new List<Entity>();

        actions = new (string, float)[1];
        actions[0] = ("Move", UnityEngine.Random.Range(0.5f, 1.5f));
    }

    public override float GetRandomTime() {
        return UnityEngine.Random.Range(4f, 7f);
    }
    public override void Move(GameObject current) {
        var ctp = current.transform.position;
        var ttp = field.player.transform.position;

        current.transform.position = nextPos;

        // RaycastHit hit;
        
        // if (Physics.Linecast (ctp, ttp, out hit)) {
        //     if (hit.transform.tag == "Player") {
        //         current.transform.position = ttp + new Vector3(0,3,0);
        //     }           
        // }

        nextPos = GetRandomPos();
    }
}       