using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerState
{
    WaitingToBeSeated,
    FollowingWaiterToTable,
    AtTable,
    Leaving
};

public class Customer : MonoBehaviour
{
    public float MoodAdjustWaitingToBeSeated = -0.1f;
    public float MoodAdjustFollowingWaiterToTable = -0.1f;
    public float MoodAdjustAtTable = 0.0f;

    public float MoveSpeed = 1.0f;

    public Transform MoveTarget;
    public Table AtTable;

    public float StartFollowDistance = 1.0f;
    public float StopFollowDistance = 0.5f;

    CustomerState CurrentState = CustomerState.WaitingToBeSeated;
    float Mood = 100.0f;

    bool DoFollow = false;

    public void SetState(CustomerState State)
    {
        CurrentState = State;

        if (CurrentState == CustomerState.AtTable)
        {
            StartCoroutine(GoToExit());
        }
    }

    void Update()
    {
        if (MoveTarget)
        {
            GoToMoveTarget();
        }

        UpdateMood();
    }

    void GoToMoveTarget()
    {
        Vector3 Position = transform.position;
        Vector3 MoveTargetPosition = MoveTarget.position;
        Vector3 ToMoveTarget = MoveTargetPosition - Position;
        float DistanceSq = ToMoveTarget.sqrMagnitude;

        if (DistanceSq >= (StartFollowDistance * StartFollowDistance))
        {
            DoFollow = true;
        }
        else if (DistanceSq <= (StopFollowDistance * StopFollowDistance))
        {
            DoFollow = false;
        }

        if (DoFollow)
        {
            Vector3 Direction = ToMoveTarget.normalized;
            Position += Direction * MoveSpeed * Time.deltaTime;
            transform.position = Position;
        }
    }

    void UpdateMood()
    {
        switch (CurrentState)
        {
            case CustomerState.WaitingToBeSeated:
            {
                Mood += MoodAdjustWaitingToBeSeated * Time.deltaTime;
                break;
            }
            case CustomerState.FollowingWaiterToTable:
            {
                Mood += MoodAdjustFollowingWaiterToTable * Time.deltaTime;
                break;
            }
            case CustomerState.AtTable:
            {
                Mood += MoodAdjustAtTable * Time.deltaTime;
                break;
            }
        }

        Mood = Mathf.Clamp(Mood, 0.0f, 100.0f);
    }

    IEnumerator GoToExit()
    {
        yield return new WaitForSeconds(10.0f);

        AtTable.Occupied = false;
        AtTable = null;

        GameObject Exit = GameObject.FindGameObjectWithTag("Exit");
        MoveTarget = Exit.transform;
    }
}
