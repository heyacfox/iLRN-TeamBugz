using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    public DuckStates duckState;
    Boid attachedBoid;
    Transform patrolOrigin;
    public float eatingTime = 2f;
    public Collider triggerCollider;
    public float distanceFromPatrolPointToStopReturning  = 3f;

    // Start is called before the first frame update
    void Start()
    {
        patrolOrigin = GameObject.FindGameObjectWithTag("DuckPatrolPoint").transform;
        attachedBoid = GetComponent<Boid>();
        duckState = DuckStates.waiting;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (duckState == DuckStates.waiting)
        {
            if (other.gameObject.tag == "Locust")
            {
                attachedBoid.Goal = other.transform;
                duckState = DuckStates.chasing;
                
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (duckState == DuckStates.returning || duckState == DuckStates.waiting)
        {
            if (other.gameObject.tag == "Locust")
            {
                attachedBoid.Goal = other.transform;
                duckState = DuckStates.chasing;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (duckState == DuckStates.chasing)
        {
            if (collision.collider.attachedRigidbody != null)
            {
                if (collision.collider.attachedRigidbody.gameObject.tag == "Locust")
                {
                    collision.gameObject.GetComponent<Locust>().getEaten();
                    duckState = DuckStates.eating;
                    attachedBoid.enabled = false;
                    Invoke("endEating", eatingTime);
                }
            }
        }
    }

    private void endEating()
    {
        attachedBoid.enabled = true;
        attachedBoid.Goal = patrolOrigin;
        duckState = DuckStates.returning;
    }

    private void Update()
    {
        if (duckState == DuckStates.eating || duckState == DuckStates.waiting)
        {
            transform.rotation = Quaternion.identity;
        }
        if (duckState == DuckStates.chasing)
        {
            transform.LookAt(attachedBoid.Goal);
        }
        //Your bug got eaten, pick a new bug
        if (duckState == DuckStates.chasing && attachedBoid.Goal == null)
        {
            duckState = DuckStates.returning;
        }
        if (duckState == DuckStates.returning)
        {
            if (Vector3.Distance(this.transform.position, attachedBoid.Goal.position) < distanceFromPatrolPointToStopReturning)
            {
                duckState = DuckStates.waiting;
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        //what happens when the chased bug leaves you range of view?
        //do we just chase bugs to the edge of the map?
    }
}

public enum DuckStates
{
    //Duck is waiting at its patrol point
    waiting,
    //Duck is actively pursuing a locust
    chasing,
    //Duck is eating a locust currently
    eating,
    //Duck is returning to patrol origin
    returning
}

