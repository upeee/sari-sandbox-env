using UnityEngine;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    private Collider[] handColliders;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handColliders = GetComponentsInChildren<Collider>();
    }

    // Update is called once per frame

    void FixedUpdate()
    {
        rb.linearVelocity = (target.position - transform.position) / Time.fixedDeltaTime;
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        Vector3 rotationDifferenceInDegrees = axis * angle * Mathf.Deg2Rad;
        rb.angularVelocity = rotationDifferenceInDegrees / Time.fixedDeltaTime;
    }

    public void EnableHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = true;
        }
    }

    public void EnableHandColliderDelay(float delay)
    {
        Invoke("EnableHandCollider", delay);
    }

    public void DisableHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }
    }
}
