using UnityEngine;
using UnityEngine.AI;

public class uiiarotate : MonoBehaviour
{
    private NavMeshAgent agent;
    private float rotateDuration = 3f;
    private float stopDuration = 3f;
    private float timer;
    private bool isRotating = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = rotateDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (isRotating || agent.velocity.magnitude == 0f)
                    {
                        if (timer <= 0f)
                        {
                            // stop rotating
                            isRotating = false;
                            timer = stopDuration;
                        }

                        //rotates the car
                        transform.rotation *= Quaternion.Euler(0f, 1300f * Time.deltaTime, 0f);
                    }
                    else
                    {
                        if (timer <= 0f)
                        {
                            // resume rotating
                            isRotating = true;
                            timer = rotateDuration;
                        }
                        
                    }
    }
}