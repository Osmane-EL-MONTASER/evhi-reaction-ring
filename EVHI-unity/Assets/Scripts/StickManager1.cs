using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class StickManager1 : MonoBehaviour
{
    public GameObject performanceManager;
    PerformanceManager performanceManagerScript;

    public float baseStickLength = 1.0f;

    public float minSecondsBetweenDrops = 1.0f;
    public float maxSecondsBetweenDrops = 3.0f;

    private float nextDropTime = 0.0f;

    public GameObject ScoreManager;

    private int m_stickFellCount = 10;

    public TMPro.TextMeshProUGUI gameStatusText;

    private GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
        for (int i = 0; i < nbChildren; i++)
        {
            GameObject stickParent = transform.GetChild(i).gameObject;
            GameObject stick = stickParent.transform.GetChild(0).gameObject;
            Stick2 stickScript = stick.GetComponent<Stick2>();

            stickScript.SetStickLength(baseStickLength);
        }
    
        performanceManagerScript = performanceManager.GetComponent<PerformanceManager>();
        gameState = performanceManagerScript.getGameState();
    }

    // Update is called once per frame
    void Update()
    {
        gameState = performanceManagerScript.getGameState();
        lock ("lockStats")
        {
            if (ScoreManager.GetComponent<ScoreManager>().IsStickSpeedsReady)
            {
                gameStatusText.text = "Jeu en cours... Attrapez les bâtons avec les manettes dès qu'ils tombent !";
                ScoreManager.GetComponent<ScoreManager>().IsStickSpeedsReady = false;
                ScoreManager.GetComponent<ScoreManager>().isFirstLaunch = false;

                // Change the speed of the sticks of ScoreManager.StickSpeeds
                int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
                for (int i = 0; i < nbChildren; i++)
                {
                    GameObject stick = transform.GetChild(i).gameObject;
                    Stick2 stickScript = stick.GetComponent<Stick2>();

                    stickScript.SetStickSpeed(ScoreManager.GetComponent<ScoreManager>().StickSpeeds[i]);
                }

                m_stickFellCount = 0;
            }
            else if(ScoreManager.GetComponent<ScoreManager>().currentScore + ScoreManager.GetComponent<ScoreManager>().failed < 10)
            {
                if (Time.time > nextDropTime)
                {
                    DropRandomStick();
                    nextDropTime += Random.Range(minSecondsBetweenDrops, maxSecondsBetweenDrops);
                }
            }
            else
            {
                gameStatusText.text = "Round terminé ! Envoi des données de jeu...";
                //Debug.Log("Waiting for the next stick speeds...");
            }
        }

        if (ScoreManager.GetComponent<ScoreManager>().currentScore + ScoreManager.GetComponent<ScoreManager>().failed >= 10)
        {
            // Reset the sticks to their initial position
            int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
            for (int i = 0; i < nbChildren; i++)
            {
                GameObject stickParent = transform.GetChild(i).gameObject;
                GameObject stick = stickParent.transform.GetChild(0).gameObject;
                Stick2 stickScript = stick.GetComponent<Stick2>();

                stickScript.ResetStick();
            }
        }
    }

    /// <summary>
    /// Drop a random stick from the Stick Manager.
    /// 
    /// This function is called every x seconds.
    /// </summary>
    public void DropRandomStick()
    {
        if (gameState == GameState.Playing)
        {
            int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
            int randomIndex = Random.Range(0, nbChildren); // On récupère un nombre aléatoire entre 0 et le nombre d'enfants

            GameObject stickParent = transform.GetChild(randomIndex).gameObject;
            //get child object
            GameObject stick = stickParent.transform.GetChild(0).gameObject;
            Stick2 stickScript = stick.GetComponent<Stick2>();

            stickScript.DropStick();
        }
    }
}
