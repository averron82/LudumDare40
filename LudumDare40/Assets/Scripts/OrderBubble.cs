using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    public SpriteRenderer Icon0;
    public SpriteRenderer Icon1;
    public SpriteRenderer Icon2;

    bool visible = false;

    public void Show(Meal meal)
    {
        Icon0.sprite = meal.Main.sprite;
        Icon1.sprite = meal.FirstSide.sprite;
        Icon2.sprite = meal.SecondSide.sprite;
        gameObject.SetActive(true);
        visible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        visible = false;
    }

    public bool IsVisible()
    {
        return visible;
    }
}
