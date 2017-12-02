using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    public Transform Follower;
    public Transform QueueStartPosition;

    public float ActivateDistance = 1.0f;

    private Animator MyAnimator;

    private SpriteRenderer MySpriteRenderer;

    bool Flipped = false;

    void Start()
    {
        MyAnimator = GetComponentInChildren<Animator>();
        MySpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        float Vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");

        Vector3 position = gameObject.transform.position;
        position.x += Horizontal * MoveSpeed * Time.deltaTime;
        position.y += Vertical * MoveSpeed * Time.deltaTime;

        // Set speed to 1 for any movement.
        bool movingHorizontally = Mathf.Abs(Horizontal) > 0.0f;
        if (movingHorizontally || Mathf.Abs(Vertical) > 0.0f)
        {
            MyAnimator.SetFloat("Speed", 1.0f);

            if (movingHorizontally)
            {
                gameObject.transform.localScale = new Vector2(Horizontal > 0.01f ? 1.0f : -1.0f, 1.0f);
            }
        }
        else
        {
            MyAnimator.SetFloat("Speed", 0.0f);
        }

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

        NewFollower.Leader = gameObject.transform;
        NewFollower.CurrentState = CustomerState.FollowingWaiterToTable;
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
            C.Leader = NearestTable.Chair0;
            C.StartFollowDistance = 0.1f;
            C.StopFollowDistance = 0.05f;
            C.CurrentState = CustomerState.AtTable;
            NearestTable.Occupied = true;
            Follower = null;
        }
    }
}
