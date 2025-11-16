using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float m_speed = 5.0f;
    public float m_rotationSpeed = 5.0f;
    public float m_StopDistance = 1.0f;
    public float m_DetectionDistance = 3.0f;

    public float m_SeperationWeight = 1.5f;
    public float m_AllignmentWeight = 1.0f;
    public float m_CohesionWeight = 1.0f;

    private Transform m_Target;
    private Vector3 m_Direction;
    private Vector3 combinedDirection;
    private Vector3 m_SeperationForce;
    void Start()
    {
       m_Target = FindFirstObjectByType<Player>().transform;
    }

    void Update()
    {
        if (m_Target != null)
        {
            FollowTarget(); 
        }          
    }

    private void FollowTarget()
    {
        m_SeperationForce = Vector3.zero;
        m_Direction = (m_Target.position - transform.position);
        m_Direction.z = 0;
        float distance = m_Direction.magnitude;

        var neighbours = GetNeighbours();

        if(neighbours.Length > 0)
        {
            CalculateSeperationForce(neighbours);
            ApplyAllignment(neighbours);
            ApplyCohesion(neighbours);
        }

        if (distance > m_StopDistance)
        {
            MoveTowardsTarget();
        }

        RotateTowardsTarget();
    }

    private void ApplyCohesion(Collider2D[] neighbours)
    {
        Vector3 averagePosition = Vector3.zero;
        foreach (var neighbour in neighbours)
        {
            averagePosition += neighbour.transform.position;
        }

        averagePosition /= neighbours.Length;
        var cohesionDir = (averagePosition - transform.position).normalized;

        m_SeperationForce += cohesionDir * m_CohesionWeight;
    }

    private void ApplyAllignment(Collider2D[] neighbours)
    {
        Vector3 neighboursForward = Vector3.zero;

        foreach(var neighbour in neighbours)
        {
            neighboursForward += neighbour.transform.up;
        }

        if(neighboursForward != Vector3.zero)
        {
            neighboursForward.Normalize();
        }

        m_SeperationForce += neighboursForward * m_AllignmentWeight;
    }

    private void CalculateSeperationForce(Collider2D[] neighbours)
    {
        foreach(var neighbour in neighbours)
        {
            var dir = neighbour.transform.position - transform.position;
            var distance = dir.magnitude;
            var awayDir = -dir.normalized;

            if (distance > 0)
            {
                m_SeperationForce += (awayDir / distance) * m_SeperationWeight;
            }
        }
    }

    private Collider2D[] GetNeighbours()
    {
        var enemyMask = LayerMask.GetMask("Enemy");
        return Physics2D.OverlapCircleAll(transform.position, m_DetectionDistance, enemyMask);
    }

    private void RotateTowardsTarget()
    {
        transform.up = Vector3.Lerp(transform.up, combinedDirection.normalized, m_rotationSpeed * Time.deltaTime);
    }

    private void MoveTowardsTarget()
    {
        m_Direction = m_Direction.normalized;
        combinedDirection = (m_Direction + m_SeperationForce).normalized;
        var movement = combinedDirection * m_speed * Time.deltaTime;
        transform.position += movement;
    }
}
