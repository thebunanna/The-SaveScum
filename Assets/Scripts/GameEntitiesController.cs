using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntitiesController : MonoBehaviour {
    Dictionary<string, GameObject> models;
    int cc;
    void Start() {
        cc = this.transform.childCount;
        models = new Dictionary<string, GameObject>();
        for (int i = 0; i < cc; i++) {
            var x = this.transform.GetChild(i).gameObject;
            models[x.name] = x;
        }
    }
    void Update() {

    }

    public GameObject GetModel (string name) {
        return models[name];
    }
}