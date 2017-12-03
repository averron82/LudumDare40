using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform Chair0;
    public Transform Chair1;

    public Sprite EmptyTable;
    public Sprite OneMeal;
    public Sprite TwoMeals;
    public SpriteRenderer tableRenderer;

    Interactive Interact;

    bool occupied = false;

    int FoodBeingConsumed = 0;

    public void SetOccupied()
    {
        occupied = true;
    }

    public void SetAvailable()
    {
        occupied = false;
        SetNumberOfMealsBeingConsumed(0);
        FoodBeingConsumed = 0;
    }

    public void IncrementFoodBeingConsumed()
    {
        SetNumberOfMealsBeingConsumed(++FoodBeingConsumed);
    }

    void SetNumberOfMealsBeingConsumed(int numFood)
    {
        switch (numFood)
        {
            case 0:
            {
                tableRenderer.sprite = EmptyTable;
                break;
            }
            case 1:
            {
                tableRenderer.sprite = OneMeal;
                break;
            }
            case 2:
            {
                tableRenderer.sprite = TwoMeals;
                break;
            }
        }
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

        waiter.BeSeatedBubble.SetActive(true);
        StartCoroutine(ShowBeSeated(waiter, 1.0f));

        SetOccupied();
    }

    IEnumerator ShowBeSeated(Waiter waiter, float Seconds)
    {
        yield return new WaitForSeconds(Seconds);

        waiter.BeSeatedBubble.SetActive(false);
    }
}
