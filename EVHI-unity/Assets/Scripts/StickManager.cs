using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class StickManager : MonoBehaviour
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

    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI failedText;

    public TMPro.TextMeshProUGUI stickLeftText;

    private GameState gameState;

    private List<int> remainingFall;
    private bool hasStartedGame;

    // Start is called before the first frame update
    void Start()
    {
        int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
        for (int i = 0; i < nbChildren; i++)
        {
            GameObject stickParent = transform.GetChild(i).gameObject;
            GameObject stick = stickParent.transform.GetChild(0).gameObject;
            Stick stickScript = stick.GetComponent<Stick>();

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
                stickLeftText.text = "Bâtons restants : 0 / 10. Bon courage !"; 
                ScoreManager.GetComponent<ScoreManager>().IsStickSpeedsReady = false;
                ScoreManager.GetComponent<ScoreManager>().isFirstLaunch = false;

                // Change the speed of the sticks of ScoreManager.StickSpeeds
                int nbChildren = transform.childCount; // On récupère le nombre d'enfants de l'empty "Stick Manager"
                for (int i = 0; i < nbChildren; i++)
                {
                    GameObject stick = transform.GetChild(i).GetChild(0).gameObject;
                    Stick stickScript = stick.GetComponent<Stick>();
                    stickScript.SetStickSpeed(ScoreManager.GetComponent<ScoreManager>().StickSpeeds[i]);
                    stickScript.SetStickLength(ScoreManager.GetComponent<ScoreManager>().StickLength[i]);
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
                scoreText.text = "0";
                failedText.text = "0";
                stickLeftText.text = "Plus de bâtons ! Appuyez sur le bouton pour rejouer..."; 
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
                Stick stickScript = stick.GetComponent<Stick>();

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
            if (hasStartedGame == false)
            {
                hasStartedGame = true;
                remainingFall = new List<int>();
                for (int i = 0; i < 10; i++)
                {
                    remainingFall.Add(i);
                }
                //shuffle the list
                for (int i = 0; i < remainingFall.Count; i++)
                {
                    int temp = remainingFall[i];
                    int randomIndex = Random.Range(i, remainingFall.Count);
                    remainingFall[i] = remainingFall[randomIndex];
                    remainingFall[randomIndex] = temp;
                }
                //debug log the list
                for (int i = 0; i < remainingFall.Count; i++)
                {
                    Debug.Log(remainingFall[i]);
                }
            }
            else
            {
                //pop the last element of remainingFall
                int randomIndex = remainingFall[remainingFall.Count - 1];
                remainingFall.RemoveAt(remainingFall.Count - 1);
                
                GameObject stickParent = transform.GetChild(randomIndex).gameObject;
                //get child object
                GameObject stick = stickParent.transform.GetChild(0).gameObject;
                Stick stickScript = stick.GetComponent<Stick>();

                stickScript.DropStick();
            }
        }
    }
}
