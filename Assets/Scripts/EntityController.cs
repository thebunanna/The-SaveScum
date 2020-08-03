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
    }

    public void SetMeta (Entity e) {
        this.meta = e;
    }
    public Entity GetMeta () {
        return meta;
    }
}