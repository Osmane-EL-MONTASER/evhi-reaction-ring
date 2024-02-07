using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBatonText : MonoBehaviour
{
    public Transform mainCam;
    public Transform worldSpaceCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        worldSpaceCanvas.LookAt(mainCam);
    }
}
