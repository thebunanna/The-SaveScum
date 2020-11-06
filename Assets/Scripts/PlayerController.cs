using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    float speed = 5;
    bool grounded, active;
    Rigidbody rb;
    Vector3 motionVector;
    
    private float timeCap;
    private float tp;
    private float hp;
    private float maxHp;
    private float inv;
    private float timeCost;
    public InputAction moveAction;
    public WeaponController weapon;
    public MapController map;
    public Camera cam;

    public Sprite[] sprites;
    public SpriteRenderer im;
    void Start()
    {
        timeCost = 0.5f;
        timeCap = 10;
        tp = 10;
        hp = 10;
        maxHp = 10;
        rb = GetComponent<Rigidbody>();
        moveAction.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        //tp += Time.deltaTime*10;
        if (tp > timeCap) tp = timeCap;
        inv -= Time.deltaTime;
        Vector2 mv = moveAction.ReadValue<Vector2>();
        Vector3 inputVector = new Vector3 (mv.x, 0, mv.y) * speed;
        inputVector.y = rb.velocity.y;
        rb.velocity = Vector3.Lerp(rb.velocity, inputVector, Time.deltaTime * 15);

        Vector2 p = Pointer.current.position.ReadValue();
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(p);
        var layermask = 1 << 9;
        layermask = ~layermask;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask)) {
            Vector3 dir = hit.point - transform.position;
            float x = dir.x;
            float z = dir.z;
            if (Mathf.Abs(x) > Mathf.Abs(z)) {
                if (x >= 0) {
                    im.sprite = sprites[3];
                }
                else {
                    im.sprite = sprites[2];
                }
            }
            else {
                if (z >= 0) {
                    im.sprite = sprites[1];
                }
                else {
                    im.sprite = sprites[0];
                }
            }
        }
    }
    public void NextLevel() {
        timeCap += 10;
        maxHp += 10;
        map.SendMessage("GenRooms");
    }
    public float GetCurrentTime () {
        return tp;
    }
    public float GetMaxTime() {
        return timeCap;
    }
    public bool UseTime() {
        if (TimeLeft()) {
            tp -= timeCost;
            return true;
        }
        return false;
    }
    public bool TimeLeft() {
        if (tp > timeCost) {
            return true;
        }
        return false;
    }
    ///<summary>adds time to player's time
    ///<returns><c>true</c> if time did not overflow</returns>
    ///</summary>
    public bool AddTime(float amt) {
        tp+=amt;
        if (tp > timeCap) {
            return false;
        }
        return true;
    }
    ///<summary>Use when time was incorrectly removed.</summary>
    public bool RefundTime () {
        return AddTime (timeCost);
    }

    public void UseTimeCapsule(MapController.TimeCapsule capsule) {
        //What should be rewound when reverting time?
        transform.position = capsule.pPos;
        transform.rotation = capsule.pRot;
        this.hp = capsule.hp;
    }

    public bool AddHealth (float amt) {
        hp += amt;
        if (hp > maxHp) {
            hp = maxHp;
            return true;
        }
        return false;
    }
    public void TakeDamage (float amt) {
        if (inv > 0) return;
        hp -= amt;
        inv = 0.3f;
    }
    public float GetHealth() {
        return hp;
    }
    public float GetMaxHealth() {
        return maxHp;
    }
    public void ChangeGun (int type) {
        weapon.ChangeGun(type);
    }
}
