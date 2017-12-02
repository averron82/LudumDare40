using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    public Transform Follower;
    public Transform QueueStartPosition;

    public float ActivateDistance = 1.0f;

    void Update()
    {
        float Vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");

        Vector3 position = gameObject.transform.position;
        position.x += Horizontal * MoveSpeed * Time.deltaTime;
        position.y += Vertical * MoveSpeed * Time.deltaTime;
        gameObject.transform.position = position;

        bool Activate = Input.GetButtonDown("Jump");
        if (Activate)
        {
            OnActivate();
        }
    }

    void OnActivate()
    {
        if (Follower)
        {
            AttemptSeatFollower();
        }
        else
        {
            Vector3 ToQueueStart = QueueStartPosition.position - transform.position;
            float DistanceSq = ToQueueStart.sqrMagnitude;
            if (DistanceSq < (ActivateDistance * ActivateDistance))
            {
                AttemptAcquireFollower();
            }
        }
    }

    void AttemptAcquireFollower()
    {
        if (!QueueManager.Instance)
        {
            return;
        }

        Customer NewFollower = QueueManager.Instance.PopCustomer();
        if (!NewFollower)
        {
            return;
        }

        NewFollower.MoveTarget = gameObject.transform;
        NewFollower.SetState(CustomerState.FollowingWaiterToTable);
        Follower = NewFollower.gameObject.transform;
    }

    void AttemptSeatFollower()
    {
        Table[] Tables = FindObjectsOfType<Table>();

        // Find the nearest table.
        Table NearestTable = null;
        float NearestDistSq = ActivateDistance;
        foreach (Table T in Tables)
        {
            Vector3 ToTable = T.transform.position - transform.position;
            float DistanceSq = ToTable.sqrMagnitude;
            if (DistanceSq <= NearestDistSq)
            {
                NearestTable = T;
            }
        }

        // If the table is unoccupoed, seat the follower at it.
        if (NearestTable && !NearestTable.Occupied)
        {
            Customer C = Follower.GetComponent<Customer>();
            C.MoveTarget = NearestTable.Chair0;
            C.StartFollowDistance = 0.1f;
            C.StopFollowDistance = 0.05f;
            C.SetState(CustomerState.AtTable);
            C.AtTable = NearestTable;
            NearestTable.Occupied = true;
            Follower = null;
        }
    }
}
