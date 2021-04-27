using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : MonoBehaviour
{
    DuckStates duckState;
    Boid attachedBoid;
    Vector3 patrolOrigin;
    public float eatingTime = 2f;
    public Collider triggerCollider;

    // Start is called before the first frame update
    void Start()
    {
        patrolOrigin = this.transform.position;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (duckState == DuckStates.chasing)
        {
            if (collision.collider.attachedRigidbody.gameObject.tag == "Locust")
            {
                collision.gameObject.GetComponent<Locust>().getEaten();
                duckState = DuckStates.eating;
                Invoke("bugEaten", eatingTime);
            }
        }
    }

    private void bugEaten()
    {
        duckState = DuckStates.waiting;
        //How do we check to see if there's any bugs in my collision area after I finish eating
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

