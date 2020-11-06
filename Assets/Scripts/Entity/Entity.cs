using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    // Start is called before the first frame update
    protected Vector3 position;
    protected Quaternion rotation;
    protected float actionTime;
    protected float iTime;
    protected GameObject model;
    protected FieldController field; //Not to be deep copied. Context changes
    public Entity () {}
    public Entity (Vector3 position, Quaternion rotation, GameObject model) {
        SetVals(position,rotation,model);
    }
    public void SetVals (Vector3 position, Quaternion rotation, GameObject model) {
        this.position = position;
        this.rotation = rotation;
        this.model = model;
    }

    public GameObject GetModel () {
        return model;
    }

    public Vector3 GetPosition() {
        return position;
    }
    public Quaternion GetQuaternion() {
        return rotation;
    }
    
    public virtual void OnCollision (GameObject current, GameObject other) {
        return;
    }
    public virtual void OnTrigger (GameObject current, GameObject other) {
        return;
    }
    public virtual void Update (GameObject current) {
        return;
    }

    public virtual Entity DeepCopy (Transform T) {
        return new Entity(T.position, T.rotation, this.model);
    }
    public virtual GameObject Spawn (FieldController field) {
        this.field = field;
        GameObject created = GameObject.Instantiate(model, position, rotation, field.GetDynamic());
        created.AddComponent<EntityController>();
        created.GetComponent<EntityController>().SetMeta(this);

        created.SetActive(true);
        return created;
    }

    protected bool GenRand (float chance) {
        return Random.Range(0, 1f) < chance;
    }
}
