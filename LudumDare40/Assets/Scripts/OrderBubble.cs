using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    public SpriteRenderer Icon0;
    public SpriteRenderer Icon1;
    public SpriteRenderer Icon2;

    public void Show(Meal meal)
    {
        Icon0.sprite = meal.Main.sprite;
        Icon1.sprite = meal.FirstSide.sprite;
        Icon2.sprite = meal.SecondSide.sprite;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
