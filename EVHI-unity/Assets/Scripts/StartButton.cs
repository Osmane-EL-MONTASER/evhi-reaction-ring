using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartButton : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    GameObject presser;
    AudioSource sound;
    bool isPressed;

    public GameObject performanceManager;
    private PerformanceManager performanceManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
        performanceManagerScript = performanceManager.GetComponent<PerformanceManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && performanceManagerScript.gameState == GameState.Start)
        {
            //move button down to 0.003f of its current position
            button.transform.position = button.transform.position + new Vector3(0, -0.01f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            isPressed = true;
            Debug.Log(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser && isPressed)
        {
            button.transform.position = button.transform.position + new Vector3(0, 0.01f, 0);
            onRelease.Invoke();
            sound.Play();
            isPressed = false;
        }
    }
}
