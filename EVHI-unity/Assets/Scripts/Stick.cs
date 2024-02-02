using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour {
    //Set stick initial position
    private Vector3 initialPosition;

    public float respawnTimer = 3.0f;
    public float respawnTime;

    private bool needRespawn = false;
    private bool isFalling = false;
    private bool isGrabbed = false;

    public GameObject scoreManager;

    private float stickSpeed = 1.0f;

    // Start is called before the first frame update
    void Start() {
        initialPosition = transform.position;
        scoreManager = GameObject.Find("Score Manager");
    }

    // Update is called once per frame
    void Update() {
        if (Time.time > respawnTime && needRespawn)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            transform.position = initialPosition;
            needRespawn = false;
            isFalling = false;
            isGrabbed = false;
            //Debug.Log("Respawn");
        }
    }

    public void ResetStick()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        transform.position = initialPosition;
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
            if (!isGrabbed)
                scoreManager.GetComponent<ScoreManager>().AddFailed(1);
        }
    }

    public void onGrab()
    {
        if (isFalling && !isGrabbed)
        {
            //Debug.Log("Grab");
            isGrabbed = true;
            scoreManager.GetComponent<ScoreManager>().AddScore(1);
        }
    }

    /// <summary>
    /// Drop the stick by applying a force to it.
    /// </summary>
    /// 
    /// <remarks>
    /// This function is called by the Stick Manager.
    /// </remarks>
    public void DropStick() {
        //Debug.Log("DropStick");
        // On récupère le rigidbody du stick
        Rigidbody rb = GetComponent<Rigidbody>();
        // On active le rigidbody
        rb.isKinematic = false;
        // On applique une force au stick pour le faire tomber
        rb.AddForce(Vector3.down * stickSpeed);
        isFalling = true;
    }

    /// <summary>
    /// Set the stick length.
    /// </summary>
    public void SetStickLength(float length) {
        Transform stickTransform = GetComponent<Transform>();
        Vector3 scale = stickTransform.localScale;
        
        // On modifie le scale du stick
        scale.z = length;
        // On applique le nouveau scale au stick
        stickTransform.localScale = scale;

        // Add the length difference with a base scale 1.0f to the stick's Y position
        stickTransform.position -= new Vector3(0.0f, (length - 1.0f) / 2.0f, 0.0f);
    }

    public void SetStickSpeed(float speed) {
        stickSpeed = speed;
    }
}
