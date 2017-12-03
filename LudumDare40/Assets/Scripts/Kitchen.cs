using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KitchenState
{
    WaitingForOrder,
    MenuVisible,
    PreparingOrder,
    WaitingForCollection
}

public class Kitchen : MonoBehaviour
{
    public KitchenMenu menu;

    Interactive interactive;
    Waiter interactor;

    Meal meal;

    KitchenState state;
    public KitchenState State
    {
        get { return state; }

        set
        {
            state = value;

            switch (state)
            {
                case KitchenState.MenuVisible:
                {
                    interactor.ReceiveInput = false;
                    menu.Show();
                    break;
                }
                case KitchenState.PreparingOrder:
                {
                    meal = menu.GetSelectedMeal();
                    menu.Hide();
                    StartCoroutine(CookMeal(Random.Range(2.0f, 4.0f)));
                    interactor.ReceiveInput = true;
                    break;
                }
            }
        }
    }

    void Start()
    {
        interactive = GetComponent<Interactive>();
        interactive.SetInteraction(Interact, ValidateInteraction);
        State = KitchenState.WaitingForOrder;
    }

    void Interact(Waiter waiter)
    {
        interactor = waiter;

        switch (State)
        {
            case KitchenState.WaitingForOrder:
            {
                State = KitchenState.MenuVisible;
                break;
            }
            case KitchenState.WaitingForCollection:
            {
                waiter.Meal = meal;
                State = KitchenState.WaitingForOrder;
                break;
            }
        }
    }

    bool ValidateInteraction(Waiter waiter)
    {
        if (waiter.Follower)
        {
            return false;
        }

        switch (State)
        {
            case KitchenState.WaitingForOrder:
            {
                return true;
            }
            case KitchenState.WaitingForCollection:
            {
                return waiter.Meal == null;
            }
        }

        return false;
    }

    IEnumerator CookMeal(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        State = KitchenState.WaitingForCollection;
    }
}
