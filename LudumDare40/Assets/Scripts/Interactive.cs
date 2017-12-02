using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public delegate void InteractionDelegate(Waiter waiter);
    public delegate bool ValidateInteractionDelegate(Waiter waiter);

    private InteractionDelegate onInteract;
    private ValidateInteractionDelegate validateInteraction;

    public void SetInteraction(InteractionDelegate interraction, ValidateInteractionDelegate validate = null)
    {
        onInteract = interraction;
        validateInteraction = validate;
    }

    public bool HasValidInteraction(Waiter waiter)
    {
        return onInteract != null && (validateInteraction == null || validateInteraction(waiter));
    }

    public void Interact(Waiter waiter)
    {
        onInteract.Invoke(waiter);
    }
}
