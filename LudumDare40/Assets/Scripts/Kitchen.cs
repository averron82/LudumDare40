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
    public OrderBubble orderBubble;
    public Transform chef;
    public Sprite hatchEmpty;
    public Sprite hatchReady;
    public SpriteRenderer hatch;

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
                case KitchenState.WaitingForOrder:
                {
                    if (orderBubble.IsVisible())
                    {
                        orderBubble.Hide();
                    }

                    hatch.sprite = hatchEmpty;
                    break;
                }
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

                    Vector3 chefPosition = chef.position;
                    chefPosition.y -= 0.1f;
                    chef.position = chefPosition;

                    StartCoroutine(CookMeal(Random.Range(2.0f, 4.0f)));
                    interactor.ReceiveInput = true;
                    break;
                }
                case KitchenState.WaitingForCollection:
                {
                    Vector3 chefPosition = chef.position;
                    chefPosition.y += 0.1f;
                    chef.position = chefPosition;

                    hatch.sprite = hatchReady;

                    orderBubble.Show(meal);
                    StartCoroutine(HideOrderBubble(3.0f));
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
                meal = null;
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

    IEnumerator HideOrderBubble(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (orderBubble.IsVisible())
        {
            orderBubble.Hide();
        }
    }
}
