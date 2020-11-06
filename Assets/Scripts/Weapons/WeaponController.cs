using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;


public class WeaponController : MonoBehaviour
{
    public BaseWeapon weapon;
    public InputAction fireAction;
    public FieldController field;
    Dictionary <string, GameObject> ammoTypes;
    public GameObject[] ammoGO;
    public Camera cam;
    private float fireRate = 0;
    private AudioSource audio;
    void Start() {
        weapon = new BasicPistol();
        fireRate = weapon.delay;
        fireAction.Enable();
        ammoTypes = new Dictionary<string, GameObject>();
        foreach (var x in ammoGO) {
            ammoTypes[x.name] = x;
        }
        audio = this.GetComponent<AudioSource>();
    }

    void Update() {
        Pointer p = Pointer.current;
        if (p.press.isPressed && fireRate < 0) {
            OnFire(p.position.ReadValue());
            fireRate = weapon.delay;
        }
        fireRate -= Time.deltaTime;
    }

    public void OnFire(Vector2 p)
    {    
        audio.PlayDelayed(0);
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(p);
        var layermask = 1 << 9;
        layermask = ~layermask;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask)) {
            Vector3 dir = hit.point - transform.position;
            dir.y = 0;
            List<(float, string)> res = weapon.Fire();

            foreach (var x in res) {
                

                Vector3 force = RotateVector(dir, x.Item1).normalized * weapon.speed;
                GameObject model = ammoTypes[x.Item2];
                Entity en = new Bullet(transform.position, Quaternion.identity, model, force, this.gameObject);
                GameObject e = en.Spawn(field);
                //this game object should have the rigid body component
                               
                Physics.IgnoreCollision(e.GetComponent<Collider>(), 
                    this.transform.parent.GetComponent<Collider>());                
            }
        }

        if (weapon.durability == 0) weapon = new BasicPistol();
        
    }

    private Vector3 RotateVector (Vector3 v, float deg) {
        float rad = toRad(deg);
        float x = v.x * Mathf.Cos (rad) - v.z * Mathf.Sin (rad);
        float z = v.x * Mathf.Sin (rad) + v.z * Mathf.Cos (rad);
        return new Vector3 (x, 0 , z);

    }
    private float toRad (float deg) {
        return ((float) Math.PI / 180) * deg;
    }

    public void ChangeGun(int type) {
        if (weapon.name == "Pistol") {
            switch (type) {
                case 0:
                    weapon = new Shotgun();
                    break;
                case 1:
                    weapon = new Rifle();
                    break;
                default:
                    weapon = new BasicPistol();
                break;
            }
        }
        else {
            weapon.Upgrade();
        }
    }
}