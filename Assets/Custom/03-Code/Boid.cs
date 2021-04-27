using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    private const float MAX_MULTIPLIER = 10f;
    [Range(0, MAX_MULTIPLIER)]
    public float CohesionSpeed = 1f;
    [Range(0, MAX_MULTIPLIER)]
    public float SeparationSpeed = 1f;
    [Range(0, MAX_MULTIPLIER)]
    public float AlignmentSpeed = 1f;
    [Range(0, MAX_MULTIPLIER)]
    public float AvoidanceSpeed = 1f;
    [Range(0, MAX_MULTIPLIER)]
    public float GoalSpeed = 1f;

    /// <summary>
    /// The goal this boid is moving towards, null if moving with the flock.
    /// </summary>
    public Transform Goal;

    /// <summary>
    /// The distance from the goal where the boid can be said to have reached the goal (and should stop moving).
    /// </summary>
    public float GoalDistance;

    /// <summary>
    /// The radius of a sphere where any boid colliding is said to be a neighbor.
    /// </summary>
    public float CohesionRadius;

    /// <summary>
    /// The layer mask containing colliders that are neighbors to this boid.
    /// </summary>
    public LayerMask NeighborLayer;

    /// <summary>
    /// Boids will move away from each other if the distance from their centers is less than this number.
    /// </summary>
    public float SeparationDistance;

    /// <summary>
    /// The radius of a sphere where any obstacle colliding will be avoided.
    /// </summary>
    public float AvoidanceRadius;

    /// <summary>
    /// The layer mask containing colliders that are obstacles to this boid.
    /// </summary>
    public LayerMask ObstacleLayer;

    /// <summary>
    /// The maximum speed a boid can move.
    /// </summary>
    public float MaxSpeed;

    public bool ReachedGoal { get { return Goal != null && (transform.position - Goal.position).sqrMagnitude <= GoalDistance * GoalDistance; } }

    private List<Transform> neighbors = new List<Transform>();
    private List<Transform> obstacles = new List<Transform>();

    private Vector3 cohesion;
    private Vector3 separation;
    private Vector3 alignment;
    private Vector3 toGoal;
    private Vector3 avoidance;

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
        Collider[] colliders = Physics.OverlapSphere(transform.position, CohesionRadius, ~NeighborLayer);
        neighbors.Clear();
        foreach (Collider c in colliders)
        {
            Boid b = c.GetComponent<Boid>();
            if (b != null && !b.ReachedGoal)
            {
                neighbors.Add(c.transform);
            }
        }
    }

    private void FindObstacles()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, AvoidanceRadius, ~ObstacleLayer);
        obstacles.Clear();
        foreach (Collider c in colliders)
        {
            obstacles.Add(c.transform);
        }
    }

    private void CalculateVectors()
    {
        cohesion = Vector3.zero;
        separation = Vector3.zero;
        alignment = Vector3.zero;
        toGoal = Vector3.zero;
        avoidance = Vector3.zero;

        int separationCount = 0;
        if (neighbors.Count > 0 && !ReachedGoal)
        {
            foreach (Transform boid in neighbors)
            {
                cohesion += boid.position;

                float dist = (transform.position - boid.position).magnitude;
                if (boid != transform && dist < SeparationDistance)
                {
                    separation += (transform.position - boid.position) / Mathf.Max(dist, .0001f);
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
            cohesion = cohesion.normalized;

            if (separationCount > 0)
            {
                separation /= separationCount;
                separation = separation.normalized;
            }

            alignment /= neighbors.Count;
            alignment = alignment.normalized;
        }

        foreach (Transform obstacle in obstacles)
        {
            Vector3 awayFrom = (transform.position - obstacle.position);
            avoidance += awayFrom / Mathf.Max(awayFrom.magnitude, .0001f);
        }
        if (obstacles.Count > 0)
        {
            avoidance /= obstacles.Count;
            avoidance = avoidance.normalized;
        }

        if (Goal != null && !ReachedGoal)
        {
            toGoal = (Goal.position - transform.position).normalized;
        }

        Debug.DrawLine(transform.position, cohesion + transform.position, Color.green);
        Debug.DrawLine(transform.position, separation + transform.position, Color.red);
        Debug.DrawLine(transform.position, alignment + transform.position, Color.blue);
        Debug.DrawLine(transform.position, toGoal + transform.position, Color.yellow);
        Debug.DrawLine(transform.position, avoidance + transform.position, Color.cyan);
    }

    private void FixedUpdate()
    {
        FindNeighbors();
        CalculateVectors();
        Vector3 newVelocity =
            cohesion * CohesionSpeed +
            separation * SeparationSpeed +
            alignment * AlignmentSpeed +
            avoidance * AvoidanceSpeed +
            toGoal * GoalSpeed;
        newVelocity = Vector3.ClampMagnitude(newVelocity, MaxSpeed);

        rb.AddForce(newVelocity - rb.velocity, ForceMode.VelocityChange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, CohesionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SeparationDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, AvoidanceRadius);
    }
}
