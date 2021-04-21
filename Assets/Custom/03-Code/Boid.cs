using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    public float CohesionRadius;

    public float SeparationDistance;

    public float MaxSpeed;

    private List<Transform> neighbors = new List<Transform>();

    private Vector3 cohesion;
    private Vector3 separation;
    private Vector3 alignment;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // For Testing
        rb.AddForce(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode.VelocityChange);
    }

    // Might want to change this into a coroutine if too costly to call every FixedUpdate
    private void FindNeighbors()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CohesionRadius, ~LayerMask.NameToLayer("Boid"));
        neighbors.Clear();
        foreach (Collider c in colliders)
        {
            neighbors.Add(c.transform);
        }
    }

    private void CalculateVectors()
    {
        cohesion = Vector3.zero;
        separation = Vector3.zero;
        alignment = Vector3.zero;

        int separationCount = 0;
        if (neighbors.Count > 0)
        {
            foreach (Transform boid in neighbors)
            {
                cohesion += boid.position;

                float dist = (transform.position - boid.position).sqrMagnitude;
                if (boid != transform && dist < (SeparationDistance * SeparationDistance))
                {
                    separation += (transform.position - boid.position);
                    separationCount++;
                }

                Rigidbody otherRB = boid.GetComponent<Rigidbody>();
                if (otherRB != null)
                {
                    alignment += otherRB.velocity;
                }
            }

            cohesion /= neighbors.Count;
            cohesion = cohesion - transform.position; // direction to center of mass

            if (separationCount > 0)
            {
                separation /= separationCount;
            }

            alignment /= neighbors.Count;
        }

        Debug.DrawLine(transform.position, cohesion + transform.position, Color.green);
        Debug.DrawLine(transform.position, separation + transform.position, Color.red);
        Debug.DrawLine(transform.position, alignment + transform.position, Color.blue);
    }

    private void FixedUpdate()
    {
        FindNeighbors();
        CalculateVectors();
        Vector3 newVelocity = cohesion + separation + alignment;
        Vector3.ClampMagnitude(newVelocity, MaxSpeed);

        rb.AddForce(newVelocity - rb.velocity, ForceMode.VelocityChange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CohesionRadius);
    }
}
