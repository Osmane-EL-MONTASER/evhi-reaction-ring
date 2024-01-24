using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickManager : MonoBehaviour {

    public float baseStickLength = 1.0f;

    public float minSecondsBetweenDrops = 1.0f;
    public float maxSecondsBetweenDrops = 3.0f;

    private float nextDropTime = 0.0f;
    // Start is called before the first frame update
    void Start() {
        // On rescale tous les sticks pour qu'ils aient la bonne longueur
        int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
        for (int i = 0; i < nbChildren; i++) {
            GameObject stick = transform.GetChild(i).gameObject;
            Stick stickScript = stick.GetComponent<Stick>();

            stickScript.SetStickLength(baseStickLength);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Time.time > nextDropTime)
        {
            DropRandomStick();
            nextDropTime += Random.Range(minSecondsBetweenDrops, maxSecondsBetweenDrops);
        }
 
    }

    /// <summary>
    /// Drop a random stick from the Stick Manager.
    /// 
    /// This function is called every x seconds.
    /// </summary>
    public void DropRandomStick() {
        int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
        int randomIndex = Random.Range(0, nbChildren); // On récupère un nombre aléatoire entre 0 et le nombre d'enfants

        GameObject stick = transform.GetChild(randomIndex).gameObject;
        Stick stickScript = stick.GetComponent<Stick>();

        stickScript.DropStick();
    }
}
