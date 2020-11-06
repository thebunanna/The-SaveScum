using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityController : MonoBehaviour
{
    private Entity meta;
    void Start() {

    }
    void Update() {
        if (meta == null) Destroy (this);
        meta.Update(this.gameObject);
    }

    public void SetMeta (Entity e) {
        this.meta = e;
    }
    public Entity GetMeta () {
        return meta;
    }
    void OnCollisionEnter(Collision c) {
        meta.OnCollision(this.gameObject, c.gameObject);
    }

    void OnTriggerEnter (Collider c) {
        meta.OnTrigger(this.gameObject, c.gameObject);
    }
    void Remove() {
        Destroy(this.gameObject);
    }

    public Entity DeepCopy() {
        return meta.DeepCopy(this.transform);
    }
}