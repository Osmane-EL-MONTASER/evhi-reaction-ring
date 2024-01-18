using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
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
        rb.AddForce(Vector3.down * 100.0f);
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
}
