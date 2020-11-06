using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
public class GenericEnemy : Entity {
    protected float hp, speed;
    protected List<Entity> drops;
    protected int index;
    protected (string, float)[] actions; 
    
    protected Vector3 args;
    public GenericEnemy () {}
    public GenericEnemy (Vector3 position, Quaternion rotation, 
        GameObject model, float hp, float speed) {

        base.SetVals(position,rotation,model);
        this.hp = hp;
        this.speed = speed;
        SetDefault();

    }

    public override void OnCollision(GameObject current, GameObject other) {
        if (other.tag == "Player_Proj") {
            Bullet b = (Bullet) other.GetComponent<EntityController>().GetMeta();
            
            hp -= b.GetSource().GetComponent<WeaponController>().weapon.dmg;
            other.SendMessage("Remove");
            field.player.SendMessage("AddTime", b.GetSource().GetComponent<WeaponController>().weapon.dmg / 2);
        }
        if (hp < 0) {
            this.Destroy(current);
            current.SendMessage("Remove");
        }
    }
    public virtual void Destroy (GameObject current) {
        return;
    }
    public override Entity DeepCopy(Transform T) {
        List<Entity> copydrops = new List<Entity>();
        foreach (var x in drops) copydrops.Add(x);

        (string, float)[] copyactions = new (string, float)[actions.Length];
        for (int i = 0; i < actions.Length; i++) {
            copyactions[i] = (actions[i].Item1, actions[i].Item2);
        }

        GenericEnemy en = new GenericEnemy(position, rotation, model, hp, speed);
        en.SetDrops(copydrops);
        en.SetActions(copyactions);
        en.args = new Vector3(this.args.x, this.args.y, this.args.z);

        return en;
    }

    public override GameObject Spawn(FieldController field) {
        this.field = field; 
        GameObject c = base.Spawn(field);
        return c;
    }

    public virtual void SetDefault () {
        drops = new List<Entity>();

        actions = new (string, float)[3];
        args = GetRandomPos();
        actions[0] = ("Idle", UnityEngine.Random.Range(0.3f, 2.0f));
        actions[1] = ("Idle", UnityEngine.Random.Range(0.1f, 0.5f));
        actions[2] = ("Attack", UnityEngine.Random.Range(1.0f, 2.0f));
    }

    public override void Update(GameObject current) {
        if (index >= actions.Length) index = 0;

        if (actions[index].Item2 < 0) {
            Type thisType = this.GetType();
            MethodInfo theMethod = thisType.GetMethod(actions[index].Item1);
            theMethod.Invoke(this, new object[]{current});
            actions[index].Item2 = GetRandomTime();
            index++;
        }
        else{
            actions[index].Item2 -= Time.deltaTime;
        }
    }
    
    public virtual float GetRandomTime () {
        return UnityEngine.Random.Range(0.5f, 2.5f);
    }

    public virtual void SetDrops (List <Entity> drops) {
        this.drops = drops;
    }
    public virtual void SetActions ((string, float)[] actions) {
        this.actions = actions;
    }
    public virtual void Move(GameObject current) { 
        var ctp = current.transform.position;
        var ttp = field.player.transform.position;
        RaycastHit hit;
        //If person in range, move back
        if (Physics.Linecast (ctp, ttp, out hit)) {
            if (hit.transform.tag == "Player") {
                if (Physics.Raycast (ctp, (ctp-ttp).normalized, out hit, 3.0f)) {
                    current.transform.position = hit.point + new Vector3 (0,1,0);
                }
                else {
                    current.transform.position = (ctp-ttp).normalized * 3 + new Vector3 (0,1,0);
                }
            }           
        }
        return;
    }
    public virtual void Idle(GameObject current) {
        //Don't do anything :)
        return;
    }
    public virtual void Attack(GameObject current) { 
        EnemyMissile mis = new EnemyMissile(current.transform.position, 
                current.transform.rotation, field.gec.GetModel("Bullet2"), Vector3.zero, current, null);
        GameObject spawn = mis.Spawn(field);
        Vector3 dir = new Vector3 (UnityEngine.Random.Range(-1.0f, 1.0f), 0, UnityEngine.Random.Range(-1.0f,1.0f));
        spawn.GetComponent<Rigidbody>().AddForce(dir * 100);
        spawn.transform.rotation = Quaternion.LookRotation(dir);

        Physics.IgnoreCollision(spawn.GetComponent<Collider>(), 
                    current.GetComponent<Collider>());  
        //Summon missiles that seek target based on ray        
        return;
    }

    protected Vector3 GetRandomPos() {
        return new Vector3 (UnityEngine.Random.Range(-4,4), 3f , UnityEngine.Random.Range(-4,4));
    }

    protected IEnumerator MoveObject(Vector3 source, Vector3 target, float speed, Transform T)
    {        
        float stime = Time.time;
        while(stime > Time.time - 3)
        {
            T.position = Vector3.MoveTowards(source, target, Time.deltaTime * speed);
            yield return null;
        }
    }

    protected IEnumerator Explode (float time, GameObject current) {
        float t = 0;
        while (t < time) {
            t += Time.deltaTime;

            foreach (Transform child in current.transform) {
                var r = child.GetComponent<Renderer>();
                if (r != null) {
                    r.material.color = Color.Lerp (r.material.color, Color.red, t / time);
                }
            }
            yield return null;
        }        

        Collider[] hc = Physics.OverlapSphere(current.transform.position, 1);
        foreach (var x in hc) {
            if (x.tag == "Player") x.SendMessage("TakeDamage", 2);
            var rb = x.GetComponent<Rigidbody>();
            if (rb != null) rb.AddExplosionForce(1000, current.transform.position, 3, 3.0F);
        }
        current.SendMessage("Remove");
    }
}