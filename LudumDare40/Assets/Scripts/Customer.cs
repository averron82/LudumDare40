using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    public Transform Leader;
    public float StartFollowDistance = 1.0f;
    public float StopFollowDistance = 0.5f;

    bool FollowLeader = false;

    void Start()
    {
    }

    void Update()
    {
        if (Leader)
        {
            Vector3 Position = transform.position;
            Vector3 LeaderPosition = Leader.position;
            Vector3 ToLeader = LeaderPosition - Position;
            float DistanceSq = ToLeader.sqrMagnitude;

            if (DistanceSq >= StartFollowDistance)
            {
                FollowLeader = true;
            }
            else if (DistanceSq <= StopFollowDistance)
            {
                FollowLeader = false;
            }

            if (FollowLeader)
            {
                Vector3 Direction = ToLeader.normalized;
                Position += Direction * MoveSpeed * Time.deltaTime;
                transform.position = Position;
            }
        }
    }
}
