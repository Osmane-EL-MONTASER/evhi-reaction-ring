using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PerformanceManager : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
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
            Vector3 handPos = new Vector3();
            Vector3 centerPos = new Vector3();

            if (leftHandGrip.action.triggered){
                handPos = leftHand.transform.position;
                centerPos = leftHand.GetComponent<SphereCollider>().center;
            } else {
                handPos = rightHand.transform.position;
                centerPos = rightHand.GetComponent<SphereCollider>().center;
            }

            handPos = handPos+centerPos;   // centrer par rapport au collider

            for (int i = 0; i < stickList.Count; i++){
                Stick2 s = stickList[i].stick;
                Vector3 stick_pos = s.transform.position;   // position du stick
                Vector3 base_scale = s.transform.parent.transform.parent.transform.localScale;     // scale du stick de base
                Vector3 stick_scale = s.transform.parent.transform.localScale;
                stick_scale = new Vector3(stick_scale.x*base_scale.x, stick_scale.y*base_scale.y, stick_scale.z*base_scale.z); // mise à l'echelle du scale du stick
                
                float xdist = 0;
                float ydist = 0;
                float zdist = 0;

                if (stick_pos.x + stick_scale.x > handPos.x){
                    xdist = stick_pos.x + stick_scale.x - handPos.x;
                } else if (stick_pos.x - stick_scale.x < handPos.x){
                    xdist = handPos.x - stick_pos.x - stick_scale.x;
                }

                if (stick_pos.y + stick_scale.y > handPos.y){
                    ydist = stick_pos.y + stick_scale.y - handPos.y;
                } else if (stick_pos.y - stick_scale.y < handPos.y){
                    ydist = handPos.y - stick_pos.y - stick_scale.y;
                }

                if (stick_pos.z + stick_scale.z > handPos.z){
                    zdist = stick_pos.z + stick_scale.z - handPos.z;
                } else if (stick_pos.z - stick_scale.z < handPos.z){
                    zdist = handPos.z - stick_pos.z - stick_scale.z;
                }

                float dist = Mathf.Sqrt(xdist*xdist + ydist*ydist + zdist*zdist);

                // si la sélection est plus proche que la précédente, mettre à jour
                if (dist <= 0.2f && dist < stickList[i].last_dist){
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
