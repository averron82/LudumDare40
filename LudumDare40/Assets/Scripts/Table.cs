using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform Chair0;
    public Transform Chair1;

    Interactive Interact;

    bool occupied = false;

    public void SetOccupied()
    {
        occupied = true;
    }

    public void SetAvailable()
    {
        occupied = false;
    }

    void Start()
    {
        Interact = GetComponent<Interactive>();
        Interact.SetInteraction(CustomersTakeSeats, ValidateInteraction);
    }

    bool ValidateInteraction(Waiter waiter)
    {
        return !occupied && waiter.Follower;
    }

    void CustomersTakeSeats(Waiter waiter)
    {
        Customer customer = waiter.Follower.GetComponent<Customer>();
        customer.MoveTarget = Chair0;
        customer.StartFollowDistance = 0.1f;
        customer.StopFollowDistance = 0.01f;
        customer.SetState(CustomerState.ConsideringOrder);
        customer.AtTable = this;

        if (customer.PlusOne)
        {
            customer.PlusOne.MoveTarget = Chair1;
            customer.PlusOne.StartFollowDistance = 0.1f;
            customer.PlusOne.StopFollowDistance = 0.01f;
            customer.PlusOne.AtTable = this;
        }

        waiter.Follower = null;
        SetOccupied();
    }
}
