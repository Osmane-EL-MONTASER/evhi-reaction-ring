using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

//Enum for gamestate
public enum GameState
{
    Start,
    Playing,
    End
}

public class PerformanceManager : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public InputActionProperty leftHandGrip;
    public InputActionProperty rightHandGrip;

    public List<float> listPerf = new List<float>();
    public List<int> stickOrder = new List<int>();
    public List<GameObject> listSticks = new List<GameObject>();

    public int stickAmount = 10;
    private int stickFellCount = 0;

    public float perfAlpha; //distance
    public float perfBeta; //exponential factor

    public GameState gameState;
    public string perfType;

    public GameObject stickManager;

    public ActionBasedController rightHandController;
    public ActionBasedController leftHandController;
    public InputHelpers.Button button;

    public GameObject scoreManager;

    public GameObject leftRayInteractor;
    public GameObject rightRayInteractor;

    public GameObject testStick;
    public GameObject testText;

    // Start is called before the first frame update
    void Start()
    {
        leftHandController = leftHand.GetComponent<ActionBasedController>();
        rightHandController = rightHand.GetComponent<ActionBasedController>();
        gameState = GameState.Start;
        stickManager = GameObject.Find("Stick Manager");
        //Children of the stick manager are the scaleTarget, and its child is the stick with the collider
        for (int i = 0; i < stickManager.transform.childCount; i++)
        {
            listSticks.Add(stickManager.transform.GetChild(i).GetChild(0).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float pressedLeft;
        float pressedRight;
        //read the grip button of the controllers
        pressedLeft = leftHandGrip.action.ReadValue<float>();
        pressedRight = rightHandGrip.action.ReadValue<float>();
        if (pressedLeft == 1 && gameState == GameState.Playing)
            updateFallingStickValues(true);
        if (pressedRight == 1 && gameState == GameState.Playing)
            updateFallingStickValues(false);
        
        if (pressedLeft == 1 && gameState != GameState.Playing)
            getPerfForTestStick(true);
        if (pressedRight == 1 && gameState != GameState.Playing)
            getPerfForTestStick(false);
        if (gameState == GameState.End)
        {
            if(scoreManager.GetComponent<ScoreManager>().algoHasStarted == true)
            {
                bool hasFinished;
                lock ("performances")
                {
                    hasFinished = !scoreManager.GetComponent<ScoreManager>().algoIsRunning;
                }
                if (hasFinished == true)
                {
                    setGameState(GameState.Start);
                    lock ("performances")
                    {
                        scoreManager.GetComponent<ScoreManager>().algoHasStarted = false;
                    }
                }
            }
        }
    }

    public void updateFallingStickValues(bool isLeftHand)
    {
        foreach (GameObject stick in listSticks)
        {
            Stick s = stick.GetComponent<Stick>();
            if (isLeftHand)
                s.updateDistOnGrabLeft(leftHand.transform.position);
            else
                s.updateDistOnGrabRight(rightHand.transform.position);
        }
    }

    public void getPerfForTestStick(bool isLeftHand)
    {
        float dist;
        if (isLeftHand)
            dist = Vector3.Distance(leftHand.transform.position, testStick.GetComponent<Collider>().ClosestPoint(leftHand.transform.position));
        else
            dist = Vector3.Distance(rightHand.transform.position, testStick.GetComponent<Collider>().ClosestPoint(rightHand.transform.position));
        float perf = 0;
        if(perfType == "lin")
        {
            perf = getPerfLin(dist);
        }
        else if(perfType == "exp")
        {
            perf = getPerfExp(dist);
        }
        perf /= 2;
        testText.GetComponent<TMPro.TextMeshProUGUI>().text = perf.ToString();
    }
    public void addPerfList(float dist, int index){
        //choose getperf function depending on perfType
        float perf = 0;
        if(perfType == "lin")
        {
            perf = getPerfLin(dist);
        }
        else if(perfType == "exp")
        {
            perf = getPerfExp(dist);
            Debug.Log("perf : " + perf);
        }
        if(perf != 1) 
            listPerf.Add(perf / 2);
        else
            listPerf.Add(perf);
        
        
        stickOrder.Add(index);
        stickFellCount++;
        if (stickFellCount == stickAmount)
        {
            stickFellCount = 0;
            gameState = GameState.End;
            Debug.Log(getAllStickPerf());
            lock ("performances")
            {
                ScoreManager comp = scoreManager.GetComponent<ScoreManager>();
                comp.perfList = new List<float>(getAllStickPerf());
                setGameState(GameState.End);
            }
            listPerf = new List<float>();
        }
    }

    public List<float> getAllStickPerf()
    {
        List<float> listRes = new List<float>();
        //for i in index
        foreach(int i in stickOrder)
        {
            listRes.Add(listPerf[i]);
        }
        return listRes;
    }
    // Get performance value with linear function
    public float getPerfLin(float dist)
    {
        return Mathf.Max(-(dist / perfAlpha) + 1,0);
    }

    // Get performance value with exponential function
    public float getPerfExp(float dist)
    {
        return Mathf.Exp(-Mathf.Pow(dist/perfAlpha,perfBeta));
    }

    // Get average performance value for the game
    public float getPerformanceGame()
    {
        float totalPerf = 0;
        foreach (float perf in listPerf)
        {
            totalPerf += perf;
        }
        return totalPerf/listPerf.Count;
    }

    public GameState getGameState()
    {
        return gameState;
    }

    public void setGameState(GameState state)
    {
        gameState = state;
        if (gameState == GameState.End)
        {
            listPerf = new List<float>();
            stickOrder = new List<int>();
            rightRayInteractor.SetActive(true);
            leftRayInteractor.SetActive(true);
        }
        else if(gameState == GameState.Start)
        {
            rightRayInteractor.SetActive(true);
            leftRayInteractor.SetActive(true);
        }
        else if(gameState == GameState.Playing)
        {
            rightRayInteractor.SetActive(false);
            leftRayInteractor.SetActive(false);
        }
    }

    public void setPerformance(string type,float perfAlpha, float perfBeta)
    {
        perfType = type;
        this.perfAlpha = perfAlpha;
        this.perfBeta = perfBeta;
    }

    public void startGame()
    {
        setGameState(GameState.Playing);
    }
}
