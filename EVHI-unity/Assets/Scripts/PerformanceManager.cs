using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerformanceManager : MonoBehaviour
{
    public Transform leftHandPos;
    public Transform rightHandPos;
    public InputActionProperty leftHandGrip;

    private List<float> gripPos = new List<float>();

    private List<(Stick stick, float last_dist, float time)> stickList = new List<(Stick stick, float last_dist, float time)>();

    private List<(Vector2 param, float dist, float time)> stickPos = new List<(Vector2 param, float dist, float time)>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHandGrip.action.ReadValue<float>() > 0.1f){
            Vector3 handPos = leftHandPos.position;
            for (int i = 0; i < stickList.Count; i++){
                Stick s = stickList[i].stick;
                Transform t = s.transform;
                float xdist = 0;
                float ydist = 0;
                float zdist = 0;

                if (t.position.x + t.localScale.x > handPos.x){
                    xdist = t.position.x - handPos.x;
                } else if (t.position.x - t.localScale.x < handPos.x){
                    xdist = handPos.x - t.position.x;
                }

                if (t.position.y + t.localScale.y > handPos.y){
                    ydist = t.position.y - handPos.y;
                } else if (t.position.y - t.localScale.y < handPos.y){
                    ydist = handPos.y - t.position.y;
                }

                if (t.position.z + t.localScale.z > handPos.z){
                    zdist = t.position.z - handPos.z;
                } else if (t.position.z - t.localScale.z < handPos.z){
                    zdist = handPos.z - t.position.z;
                }

                float dist = Mathf.Sqrt(xdist*xdist + ydist*ydist + zdist*zdist);

                if (dist < stickList[i].last_dist){
                    stickList[i] = (s, dist, Time.time);
                    Debug.Log("Distance: " + dist);
                }
            }
        }
    }

    public void addFallingStick(Stick stick, float time){
        stickList.Add((stick, 1000000f, 0f));
    }
}
