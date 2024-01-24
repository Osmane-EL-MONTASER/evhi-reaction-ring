using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float currentScore = 0;
    public float failed = 0;

    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI failedText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddScore(float n)
    {
        currentScore += n;
        scoreText.text = currentScore.ToString();
    }

    public void AddFailed(float n)
    {
        failed += n;
        failedText.text = failed.ToString();
    }
}
