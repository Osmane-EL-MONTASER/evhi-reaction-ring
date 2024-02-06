using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick2 : MonoBehaviour {

    private PerformanceManager performanceManagerScript;

    public float respawnTimer = 3.0f;
    public float respawnTime;

    private bool needRespawn = false;
    private bool isFalling = false;
    private bool isGrabbed = false;

    public GameObject scoreManager;

    private float stickSpeed = 1.0f;
    private float fallTime;

    private GameObject perfManager;

    // Start is called before the first frame update
    void Start() {
        scoreManager = GameObject.Find("Score Manager");
        perfManager = GameObject.Find("Performance Manager");
    }

    // Update is called once per frame
    void Update() {
        if (Time.time > respawnTime && needRespawn)
        {
            ResetStick();
        }
    }

    public void ResetStick()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0, -1, 0);
        needRespawn = false;
        isFalling = false;
        isGrabbed = false;
    }

    // When the stick touch trigger box, make it invisible and make it visible again after 3 seconds to its initial position
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fall Zone")
        {
            //Debug.Log("Fall");
            respawnTime = respawnTimer + Time.time;
            needRespawn = true;
            if (!isGrabbed){
                scoreManager.GetComponent<ScoreManager>().AddFailed(1);
                perfManager.GetComponent<PerformanceManager>().addPerfList(this);
            }
        }
    }

    public void onGrab()
    {
        if (isFalling && !isGrabbed)
        {
            //Debug.Log("Grab");
            isGrabbed = true;
            scoreManager.GetComponent<ScoreManager>().AddScore(1);
            transform.position = new Vector3(-5,-5,-5);
            respawnTime = respawnTimer + Time.time;
            needRespawn = true;
        }
        //SetStickLength(2.0f); //test size change when grabbed
    }

    /// <summary>
    /// Drop the stick by applying a force to it.
    /// </summary>
    /// 
    /// <remarks>
    /// This function is called by the Stick Manager.
    /// </remarks>
    public void DropStick() {
        Debug.Log("DropStick");
        // On récupère le rigidbody du stick
        Rigidbody rb = GetComponent<Rigidbody>();
        // On active le rigidbody
        rb.isKinematic = false;
        // On applique une force au stick pour le faire tomber
        rb.AddForce(Vector3.down * stickSpeed);
        isFalling = true;
        fallTime = Time.time;
        perfManager.GetComponent<PerformanceManager>().addFallingStick(this);
    }

    /// <summary>
    /// Set the stick length.
    /// </summary>
    public void SetStickLength(float length) {
        Transform stickTransform = GetComponent<Transform>();
        //Get parent transform
        Transform parentTransform = stickTransform.parent;

        Vector3 scale = parentTransform.localScale;
        
        // On modifie le scale du stick
        scale.y = length;
        // On applique le nouveau scale au stick
        parentTransform.localScale = scale;
    }

    public void SetStickSpeed(float speed) {
        stickSpeed = speed;
    }

    public bool getIsGrabbed(){
        return isGrabbed;
    }

    public float getFallTime(){
        return fallTime;
    }
}
