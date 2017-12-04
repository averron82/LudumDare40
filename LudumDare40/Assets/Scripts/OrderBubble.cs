using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderBubble : MonoBehaviour
{
    public SpriteRenderer Icon0;
    public SpriteRenderer Icon1;
    public SpriteRenderer Icon2;

    bool visible = false;
    bool flipped = false;

    public void Show(Meal meal, bool flip = false)
    {
        if (flip != flipped)
        {
            Vector3 scale = gameObject.transform.localScale;
            scale.x = -scale.x;
            gameObject.transform.localScale = scale;
            flipped = flip;
        }

        if (flipped)
        {
            Icon0.sprite = meal.SecondSide.sprite;
            Icon1.sprite = meal.FirstSide.sprite;
            Icon2.sprite = meal.Main.sprite;
        }
        else
        {
            Icon0.sprite = meal.Main.sprite;
            Icon1.sprite = meal.FirstSide.sprite;
            Icon2.sprite = meal.SecondSide.sprite;
        }

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
