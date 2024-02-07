using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public List<GameObject> listSticks = new List<GameObject>();

    public int stickAmount = 10;
    private int stickFellCount = 0;

    public float perfAlpha; //distance
    public float perfBeta; //exponential factor

    public GameState gameState;
    public string perfType;

    private GameObject stickManager;

    public ActionBasedController rightHandController;
    public ActionBasedController leftHandController;
    public InputHelpers.Button button;

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
        
        if (pressedLeft == 1)
            updateFallingStickValues(true);
        if (pressedRight == 1)
            updateFallingStickValues(false);
    }

    public void updateFallingStickValues(bool isLeftHand) {
        if (gameState == GameState.Playing)
        {
            foreach (GameObject stick in listSticks)
            {
                Stick2 s = stick.GetComponent<Stick2>();
                if(isLeftHand)
                    s.updateDistOnGrabLeft(leftHand.transform.position);
                else
                    s.updateDistOnGrabRight(rightHand.transform.position);
            }
        }
    }

    public void addPerfList(float dist){
        //choose getperf function depending on perfType
        float perf = 0;
        if(perfType == "lin")
        {
            perf = getPerfLin(dist);
        }
        else if(perfType == "exp")
        {
            perf = getPerfExp(dist);
        }
        listPerf.Add(perf);
        stickFellCount++;
        if (stickFellCount == stickAmount)
        {
            stickFellCount = 0;
            gameState = GameState.End;
            Debug.Log("Performance for Game:" + getPerformanceGame());
        }
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

    public float getPerformanceGame()
    {
        float totalPerf = 0;
        foreach (float perf in listPerf)
        {
            totalPerf += perf;
        }
        return totalPerf;
    }

    public GameState getGameState()
    {
        return gameState;
    }

    public void setGameState(GameState state)
    {
        gameState = state;
    }
}
