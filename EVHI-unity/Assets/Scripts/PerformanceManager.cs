using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerformanceManager : MonoBehaviour
{
    public Transform leftHandPos;
    public Transform rightHandPos;
    public InputActionProperty leftHandGrip;
    public InputActionProperty rightHandGrip;

    private List<float> gripPos = new List<float>();

    private List<(Stick2 stick, float last_dist, float time)> stickList = new List<(Stick2 stick, float last_dist, float time)>();

    private List<(float dist, float time)> perfList = new List<(float dist, float time)>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHandGrip.action.triggered || rightHandGrip.action.triggered){
            Vector3 handPos = leftHandPos.position;
            for (int i = 0; i < stickList.Count; i++){
                Stick2 s = stickList[i].stick;
                Transform t = s.transform;
                float xdist = 0;
                float ydist = 0;
                float zdist = 0;

                if (t.position.x + 2*t.localScale.x > handPos.x){
                    xdist = t.position.x - handPos.x;
                } else if (t.position.x - 2*t.localScale.x < handPos.x){
                    xdist = handPos.x - t.position.x;
                }

                if (t.position.y + 2*t.localScale.y > handPos.y){
                    ydist = t.position.y - handPos.y;
                } else if (t.position.y - 2*t.localScale.y < handPos.y){
                    ydist = handPos.y - t.position.y;
                }

                if (t.position.z + 2*t.localScale.z > handPos.z){
                    zdist = t.position.z - handPos.z;
                } else if (t.position.z - 2*t.localScale.z < handPos.z){
                    zdist = handPos.z - t.position.z;
                }

                float dist = Mathf.Sqrt(xdist*xdist + ydist*ydist + zdist*zdist);

                if (dist <= 3f && dist < stickList[i].last_dist){
                    stickList[i] = (s, dist, s.getFallTime() - Time.time);
                }
            }
        }
    }

    public void reset(){
        stickList.Clear();
        perfList.Clear();
    }

    public void addFallingStick(Stick2 stick){
        stickList.Add((stick, 1000000f, 0));
    }

    public void addPerfList(Stick2 stick){
        for (int i = 0; i < stickList.Count; i++){
            if (stickList[i].stick == stick){
                Stick2 s = stickList[i].stick;

                if (s.getFallTime() > Time.time) {
                    perfList.Add((0, s.getFallTime() - Time.time));
                } else {
                    perfList.Add((stickList[i].last_dist, stickList[i].time));
                }

                stickList.RemoveAt(i);
                break;
            }
        }
    }
}
