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
        customer.StopFollowDistance = 0.05f;
        customer.State = CustomerState.ConsideringOrder;
        customer.table = this;

        if (customer.PlusOne)
        {
            customer.PlusOne.MoveTarget = Chair1;
            customer.PlusOne.StopFollowDistance = 0.05f;
            customer.PlusOne.table = this;
        }

        waiter.Follower = null;
        SetOccupied();
    }
}
