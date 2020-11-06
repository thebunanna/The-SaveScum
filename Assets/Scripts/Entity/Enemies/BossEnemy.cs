using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
public class BossEnemy : GenericEnemy {
    protected string[] possible = {"Attack1"};//, "Attack2", "Attack3", "Heal", "Shield"};
    (int, int) attack1val;
    public BossEnemy (Vector3 position, Quaternion rotation, 
        GameObject model, float hp, float speed) {
        base.SetVals(position,rotation,model);
        this.hp = hp;
        
    }

    public override float GetRandomTime() {
        return UnityEngine.Random.Range(5f, 10f);
    }

    public override void Update(GameObject current) {
        if (this.actions == null) SetDefault();
        if (index >= actions.Length) index = 0;

        if (actions[index].Item2 < 0) {
            Type thisType = this.GetType();
            MethodInfo theMethod = thisType.GetMethod(actions[index].Item1);
            theMethod.Invoke(this, new object[]{current});
            actions[index] = (possible[UnityEngine.Random.Range(0, possible.Length)], GetRandomTime());
            index++;
        }
        else{
            actions[index].Item2 -= Time.deltaTime;
        }
    }

    public override Entity DeepCopy(Transform T) {
        List<Entity> copydrops = new List<Entity>();
        foreach (var x in drops) copydrops.Add(x);

        (string, float)[] copyactions = new (string, float)[actions.Length];
        for (int i = 0; i < actions.Length; i++) {
            copyactions[i] = (actions[i].Item1, actions[i].Item2);
        }

        var en = new BossEnemy(position, rotation, model, hp, speed);
        en.SetDrops(copydrops);
        en.SetActions(copyactions);
        en.attack1val = this.attack1val;
        return en;
    }

    public override void SetDefault () {
        attack1val = Helper1();

        drops = new List<Entity>();
        drops.Add(new HealthPack(position, rotation, field.gec.GetModel("Heal")));
        drops.Add(new TimePack(position, rotation, field.gec.GetModel("Time")));
        drops.Add(new GunPack(position, rotation, field.gec.GetModel("GPack")));
        drops.Add(new NextLevel(position, rotation, field.gec.GetModel("LevelUp")));
        actions = new (string, float)[1];
        
        args = GetRandomPos();
        actions[0] = ("Attack1", UnityEngine.Random.Range(0.3f, 2.0f));
    //     actions[1] = ("Attack2", UnityEngine.Random.Range(0.1f, 0.5f));
    //     actions[2] = ("Attack3", UnityEngine.Random.Range(1.0f, 2.0f));
    }

    public void Attack1 (GameObject current) {
        Vector3 dir = new Vector3 (UnityEngine.Random.Range(-1.0f, 1.0f), 0, UnityEngine.Random.Range(-1.0f,1.0f));        
        (int, int) ignore = attack1val;
        for (int i = 0; i < 30; i++)
        {
            if (ignore.Item1 <= i && ignore.Item2 >= i) continue;
            int a = 360 / 30 * i;
            Vector3 pos = RandomCircle(current.transform.position, 1.0f ,a);
            Vector3 dire = (pos - current.transform.position).normalized;
            Bullet mis = new EnemyBullet(pos, 
                Quaternion.LookRotation(dire), field.gec.GetModel("Bullet2"), dire * 120, current);
            GameObject spawn = mis.Spawn(field);
            spawn.GetComponent<Rigidbody>().AddForce(dire * 120);
            Physics.IgnoreCollision(spawn.GetComponent<Collider>(), 
                    current.GetComponent<Collider>()); 

        }
        attack1val = Helper1();
    }
    private (int, int) Helper1 () {
        int x = UnityEngine.Random.Range(0, 30 - 5);
        return (x, x+5);
    }
    Vector3 RandomCircle(Vector3 center, float radius,int a)
    {
        float ang = a;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
    }

    public override void Destroy(GameObject current) {
        foreach (var x in drops) {
            x.Spawn(field);
        }
    }
}