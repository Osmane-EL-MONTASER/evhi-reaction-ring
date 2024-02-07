using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle linToggle;
    public Toggle expToggle;

    private bool linToggleOldValue = false;
    private bool expToggleOldValue = true;

    public Slider perfAlphaValue;
    public Slider perfBetaValue;

    public GameObject perfAlphaGo;
    public GameObject perfBetaGo;
    public GameObject perfBetaParams;

    private TMP_Text perfAlphaText;
    private TMP_Text perfBetaText;

    public int nPoints = 10;
    public int maxX = 3;

    public int additionalpoints = 5;
    public GameObject perfGraph;
    private WindowGraph windowGraph;

    public GameObject performanceManager;
    private PerformanceManager performanceManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        windowGraph = perfGraph.GetComponent<WindowGraph>();
        perfAlphaText = perfAlphaGo.GetComponent<TMP_Text>();
        perfBetaText = perfBetaGo.GetComponent<TMP_Text>();
        performanceManagerScript = performanceManager.GetComponent<PerformanceManager>();
        onPerfSettingsUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPerfChoice()
    {
        if(linToggle.isOn && linToggleOldValue == false)
        {
            expToggle.isOn = false;
            linToggleOldValue = true;
            expToggleOldValue = false;
        }
        else if(expToggle.isOn && expToggleOldValue == false)
        {
            linToggle.isOn = false;
            expToggleOldValue = true;
            linToggleOldValue = false;
        }
        else if(!linToggle.isOn && linToggleOldValue == true)
        {
            expToggle.isOn = true;
            expToggleOldValue = true;
            linToggleOldValue = false;
        }
        else if(!expToggle.isOn && expToggleOldValue == true)
        {
            linToggle.isOn = true;
            linToggleOldValue = true;
            expToggleOldValue = false;
        }
    }

    public void onPerfSettingsUpdate()
    {
        if(linToggle.isOn)
        {
            perfBetaParams.SetActive(false);
        }
        else
        {
            perfBetaParams.SetActive(true);
        }
        perfAlphaText.text = perfAlphaValue.value.ToString("F2");
        perfBetaText.text = perfBetaValue.value.ToString("F2");
        List<float> valueList = new List<float>(); //value list we send to the graph
        if (linToggle.isOn)
        {
            for(int i = 0; i < nPoints; i++)
            {
                float dist = i * (perfAlphaValue.value / (nPoints - 1));
                valueList.Add(Mathf.Max(-(dist / perfAlphaValue.value) + 1, 0));
            }
            for(int i = 0; i < additionalpoints; i++)
            {
                valueList.Add(0);
            }
        }
        else if(expToggle.isOn)
        {
            for(int i = 0; i < nPoints + additionalpoints; i++)
            {
                float dist = i * (perfAlphaValue.value / (nPoints - 1));
                valueList.Add(Mathf.Exp(-Mathf.Pow(dist / perfAlphaValue.value, perfBetaValue.value)));
            }
        }
        windowGraph.ClearGraph();
        windowGraph.ShowGraph(valueList);
        performanceManagerScript.setPerformance(linToggle.isOn ? "lin" : "exp", perfAlphaValue.value, perfBetaValue.value);
    }

    public void resetDefaultValues()
    {
        expToggle.isOn = true;
        linToggle.isOn = false;
        perfAlphaValue.value = 0.1f;
        perfBetaValue.value = 2.5f;
        onPerfSettingsUpdate();
    }
}
