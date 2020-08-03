using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 5;
    bool grounded, active;
    Rigidbody rb;
    Vector3 motionVector;
    
    private float timeCap;
    private float cTime;
    private float timeCost;
    void Start()
    {
        timeCost = 1;
        timeCap = 10;
        cTime = 0;
        Time.fixedDeltaTime = 1 / 100f;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        cTime += Time.deltaTime/2;
        if (cTime > timeCap) cTime = timeCap;
        
        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputVector *= speed;
        inputVector.y = rb.velocity.y;
        rb.velocity = Vector3.Lerp(rb.velocity, inputVector, Time.deltaTime * 15);
    }
    
    public float GetCurrentTime () {
        return cTime;
    }
    public float GetMaxTime() {
        return timeCap;
    }
    public bool UseTime() {
        if (TimeLeft()) {
            cTime -= timeCost;
            return true;
        }
        return false;
    }
    public bool TimeLeft() {
        if (cTime > timeCost) {
            return true;
        }
        return false;
    }
    ///<summary>adds time to player's time
    ///<returns><c>true</c> if time did not overflow</returns>
    ///</summary>
    public bool AddTime(float amt) {
        cTime+=amt;
        if (cTime > timeCap) {
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
        
    }
}
