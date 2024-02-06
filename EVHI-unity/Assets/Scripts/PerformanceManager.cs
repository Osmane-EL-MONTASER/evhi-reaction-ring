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
            float colliderRadius;

            if (leftHandGrip.action.triggered){
                handPos = leftHand.transform.position;
                centerPos = leftHand.GetComponent<SphereCollider>().center;
                colliderRadius = leftHand.GetComponent<SphereCollider>().radius;
            } else {
                handPos = rightHand.transform.position;
                centerPos = rightHand.GetComponent<SphereCollider>().center;
                colliderRadius = rightHand.GetComponent<SphereCollider>().radius;
            }

            handPos = handPos+centerPos;   // centrer par rapport au collider
            int nearest = -1;
            float nearest_dist = 1000000f;

            for (int i = 0; i < stickList.Count; i++){
                Stick2 s = stickList[i].stick;

                // Si un stick a été attrapé, l'ajouter à la liste des performances
                if (s.getIsGrabbed()) {
                    perfList.Add((0, s.getFallTime() - Time.time));
                    stickList.RemoveAt(i);
                    return;
                }

                Vector3 stick_pos = s.transform.position;   // position du stick
                Vector3 base_scale = s.transform.parent.transform.parent.transform.localScale;     // scale du stick de base
                Vector3 stick_scale = s.transform.parent.transform.localScale;
                stick_scale = new Vector3(stick_scale.x*base_scale.x/2, stick_scale.y*base_scale.y, stick_scale.z*base_scale.z/2); // mise à l'echelle du scale du stick
                
                float xdist = 0;
                float ydist = 0;
                float zdist = 0;

                if (stick_pos.x + stick_scale.x < handPos.x){
                    xdist = handPos.x - stick_pos.x - stick_scale.x;
                } else if (stick_pos.x - stick_scale.x > handPos.x){
                    xdist = stick_pos.x - stick_scale.x - handPos.x;
                }

                if (stick_pos.y + stick_scale.y < handPos.y){
                    ydist = handPos.y - stick_pos.y - stick_scale.y;
                } else if (stick_pos.y - stick_scale.y > handPos.y){
                    ydist = stick_pos.y - stick_scale.y - handPos.y;
                }

                if (stick_pos.z + stick_scale.z < handPos.z){
                    zdist = handPos.z - stick_pos.z - stick_scale.z;
                } else if (stick_pos.z - stick_scale.z > handPos.z){
                    zdist = stick_pos.z - stick_scale.z - handPos.z;
                }

                // en prenant en compte le rayon du collider
                // comportement étrange, à revoir
                // if (stick_pos.x + stick_scale.x < handPos.x){
                //    if (stick_pos.x + stick_scale.x < handPos.x - colliderRadius){
                //        xdist = handPos.x - colliderRadius - stick_pos.x - stick_scale.x;
                //    }
                // } else if (stick_pos.x - stick_scale.x > handPos.x){
                //    if (stick_pos.x - stick_scale.x > handPos.x + colliderRadius){
                //        xdist = stick_pos.x + colliderRadius - stick_scale.x - handPos.x;
                //    }
                // }

                // if (stick_pos.y + stick_scale.y < handPos.y){
                //    if (stick_pos.y + stick_scale.y < handPos.y - colliderRadius){
                //        ydist = handPos.y - colliderRadius - stick_pos.y - stick_scale.y;
                //    }
                // } else if (stick_pos.y - stick_scale.y > handPos.y){
                //    if (stick_pos.y - stick_scale.y > handPos.y + colliderRadius){
                //        ydist = stick_pos.y + colliderRadius - stick_scale.y - handPos.y;
                //    }
                // }

                // if (stick_pos.z + stick_scale.z < handPos.z){
                //    if (stick_pos.z + stick_scale.z < handPos.z - colliderRadius){
                //        zdist = handPos.z - colliderRadius - stick_pos.z - stick_scale.z;
                //    }
                // } else if (stick_pos.z - stick_scale.z > handPos.z){
                //    if (stick_pos.z - stick_scale.z > handPos.z + colliderRadius){
                //        zdist = stick_pos.z + colliderRadius - stick_scale.z - handPos.z;
                //    }
                // }

                float dist = Mathf.Sqrt(xdist*xdist + ydist*ydist + zdist*zdist);

                // récupération du stick le plus proche de la selection
                if (dist < nearest_dist){
                    nearest = i;
                    nearest_dist = dist;
                    Debug.Log("distx: " + xdist + " disty: " + ydist + " distz: " + zdist);
                }
            }

            // si la sélection est plus proche que la précédente, mettre à jour
            if (nearest != -1){
                Stick2 s = stickList[nearest].stick;
                float dist = nearest_dist;
                float time = stickList[nearest].time;

                if (dist < stickList[nearest].last_dist){
                    stickList[nearest] = (s, dist, s.getFallTime() - Time.time);
                    Debug.Log("Stick pos: " + s.transform.position);
                    Debug.Log("Hand pos: " + handPos);
                    Debug.Log("Dist: " + dist);
                }
            }
        }
    }

    public float get_Perf(){
        stickList.Clear();
        perfList.Clear();
        
    }

    public void addFallingStick(Stick2 stick){
        stickList.Add((stick, 1000000f, 0));
    }

    public void addPerfList(Stick2 stick){
        // ajout du stick qui est tombé au "sol" dans la liste des performances
        for (int i = 0; i < stickList.Count; i++){
            if (stickList[i].stick == stick){
                Stick2 s = stickList[i].stick;
                perfList.Add((stickList[i].last_dist, stickList[i].time));
                stickList.RemoveAt(i);
                return;
            }
        }
    }
}
