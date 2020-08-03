using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    // Start is called before the first frame update
    private Vector3 position;
    private Quaternion rotation;
    private float actionTime;
    private float iTime;
    private GameObject model;

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

}
