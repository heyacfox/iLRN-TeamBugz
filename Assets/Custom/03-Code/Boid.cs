using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    private const float MAX_MULTIPLIER = 2f;
    [Range(0, MAX_MULTIPLIER)]
    public float CohesionWeight = 1f;
    [Range(0, MAX_MULTIPLIER)]
    public float SeparationWeight = 1f;
    [Range(0, MAX_MULTIPLIER)]
    public float AlignmentWeight = 1f;
    [Range(0, MAX_MULTIPLIER)]
    public float GoalWeight = 1f;

    /// <summary>
    /// The goal this boid is moving towards, null if moving with the flock.
    /// </summary>
    public Transform Goal;

    /// <summary>
    /// The speed at which a boid should *try* to move towards its goal.
    /// </summary>
    public float GoalSpeed;

    /// <summary>
    /// The distance from the goal where the boid can be said to have reached the goal (and should stop moving).
    /// </summary>
    public float GoalDistance;

    /// <summary>
    /// The radius of a sphere where any boid colliding is said to be a neighbor.
    /// </summary>
    public float CohesionRadius;

    /// <summary>
    /// Boids will move away from each other if the distance from their centers is less than this number.
    /// </summary>
    public float SeparationDistance;

    /// <summary>
    /// The maximum speed a boid can move.
    /// </summary>
    public float MaxSpeed;

    public bool ReachedGoal { get { return Goal != null && (transform.position - Goal.position).sqrMagnitude <= GoalDistance * GoalDistance; } }

    private List<Transform> neighbors = new List<Transform>();

    private Vector3 cohesion;
    private Vector3 separation;
    private Vector3 alignment;
    private Vector3 toGoal;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // For Testing
        //rb.AddForce(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode.VelocityChange);
    }

    // Might want to change this into a coroutine if too costly to call every FixedUpdate
    private void FindNeighbors()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, CohesionRadius, ~LayerMask.NameToLayer("Boid"));
        neighbors.Clear();
        foreach (Collider c in colliders)
        {

            if (c.GetComponent<Boid>() != null && !c.GetComponent<Boid>().ReachedGoal)
            {
                neighbors.Add(c.transform);
            }
        }
    }

    private void CalculateVectors()
    {
        cohesion = Vector3.zero;
        separation = Vector3.zero;
        alignment = Vector3.zero;
        toGoal = Vector3.zero;

        int separationCount = 0;
        if (neighbors.Count > 0 && !ReachedGoal)
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

        if (Goal != null && !ReachedGoal)
        {
            toGoal = (Goal.position - transform.position).normalized * GoalSpeed;
        }

        Debug.DrawLine(transform.position, cohesion + transform.position, Color.green);
        Debug.DrawLine(transform.position, separation + transform.position, Color.red);
        Debug.DrawLine(transform.position, alignment + transform.position, Color.blue);
        Debug.DrawLine(transform.position, toGoal + transform.position, Color.yellow);
    }

    private void FixedUpdate()
    {
        FindNeighbors();
        CalculateVectors();
        Vector3 newVelocity = cohesion * CohesionWeight + separation * SeparationWeight + alignment * AlignmentWeight + toGoal * GoalWeight;
        newVelocity = Vector3.ClampMagnitude(newVelocity, MaxSpeed);

        rb.AddForce(newVelocity - rb.velocity, ForceMode.VelocityChange);

        // Lock position if reached goal?
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, CohesionRadius);
    }
}
