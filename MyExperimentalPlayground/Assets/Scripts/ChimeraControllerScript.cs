using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChimeraControllerScript : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Rigidbody rigidbody;


    // Start is called before the first frame update
    void Start()
    {
        //navAgent.SetDestination(new Vector3(0, 0, 0));
        Debug.Log(navAgent.updateRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if(navAgent.updateRotation)
        {
            //Don't move. Just rotate in place then move.
        }
        else if(navAgent.updatePosition)
        {
            //Move to the location
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Im trying to yeet");
            //rigidbody.AddExplosionForce(1000, transform.position, 1000);
            rigidbody.AddForce(new Vector3(100,0, 0),ForceMode.Impulse);
            //navAgent.velocity = new Vector3(0, 100, 0);
        }
    }
}
