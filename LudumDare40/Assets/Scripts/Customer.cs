﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerState
{
    WaitingToBeSeated,
    FollowingWaiterToTable,
    ConsideringOrder,
    WaitingToPlaceOrder,
    WaitingForMeal,
    EatingMeal,
    Leaving,

    PlusOne
};

public class Customer : MonoBehaviour
{
    public float MoodAdjustWaitingToBeSeated = -0.1f;
    public float MoodAdjustFollowingWaiterToTable = -0.1f;
    public float MoodAdjustConsideringOrder = 0.0f;
    public float MoodAdjustWaitingToPlaceOrder = -0.1f;
    public float MoodAdjustWaitingForMeal = -0.1f;
    public float MoodAdjustEatingMeal = 0.0f;

    public float MoveSpeed = 1.0f;

    public Transform MoveTarget;
    public Table AtTable;
    public Customer PlusOne;

    public float StartFollowDistance = 1.0f;
    public float StopFollowDistance = 0.5f;

    CustomerState CurrentState = CustomerState.WaitingToBeSeated;
    float Mood = 100.0f;

    bool DoFollow = false;

    public void SetState(CustomerState State)
    {
        CurrentState = State;

        if (CurrentState == CustomerState.ConsideringOrder)
        {
            StartCoroutine(WantToPlaceOrder());
        }
    }

    void Start()
    {
        if ((CurrentState != CustomerState.PlusOne) && Random.Range(0.0f, 1.0f) > 0.5f)
        {
            PlusOne = Instantiate(this, transform.position, Quaternion.identity);
            PlusOne.MoveTarget = transform;
            PlusOne.CurrentState = CustomerState.PlusOne;
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
            case CustomerState.ConsideringOrder:
            {
                Mood += MoodAdjustConsideringOrder * Time.deltaTime;
                break;
            }
            case CustomerState.WaitingToPlaceOrder:
            {
                Mood += MoodAdjustWaitingToPlaceOrder * Time.deltaTime;
                break;
            }
            case CustomerState.WaitingForMeal:
            {
               Mood += MoodAdjustWaitingForMeal * Time.deltaTime;
               break;
            }
            case CustomerState.EatingMeal:
            {
                Mood += MoodAdjustEatingMeal * Time.deltaTime;
                break;
            }
        }

        Mood = Mathf.Clamp(Mood, 0.0f, 100.0f);
    }

    IEnumerator WantToPlaceOrder()
    {
        yield return new WaitForSeconds(10.0f);

        SetState(CustomerState.WaitingToPlaceOrder);
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
