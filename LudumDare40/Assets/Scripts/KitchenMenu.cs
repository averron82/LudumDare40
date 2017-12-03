using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenMenu : MonoBehaviour
{
    public Kitchen kitchen;

    public SpriteRenderer Icon0;
    public SpriteRenderer Icon1;
    public SpriteRenderer Icon2;

    int stage = 0;
    Ingredient main;
    Ingredient firstSide;
    Ingredient secondSide;

    public int Stage
    {
        get { return stage; }

        set
        {
            stage = value;

            switch (stage)
            {
                case 0:
                {
                    PopulateIcons(MealManager.Instance.Mains);
                    break;
                }
                case 1:
                {
                    PopulateIcons(MealManager.Instance.FirstSides);
                    break;
                }
                case 2:
                {
                    PopulateIcons(MealManager.Instance.SecondSides);
                    break;
                }
                case 3:
                {
                    kitchen.State = KitchenState.PreparingOrder;
                    break;
                }
            }
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

        Stage = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public Meal GetSelectedMeal()
    {
        Meal result = new Meal();
        result.Main = main;
        result.FirstSide = firstSide;
        result.SecondSide = secondSide;
        return result;
    }

    void PopulateIcons(List<Ingredient> ingredients)
    {
        Icon0.sprite = ingredients[0].sprite;
        Icon1.sprite = ingredients[1].sprite;
        Icon2.sprite = ingredients[2].sprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectIngredient(0);
            ++Stage;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectIngredient(1);
            ++Stage;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectIngredient(2);
            ++Stage;
        }
    }

    void SelectIngredient(int Index)
    {
        switch (Stage)
        {
            case 0:
            {
                main = MealManager.Instance.Mains[Index];
                break;
            }
            case 1:
            {
                firstSide = MealManager.Instance.FirstSides[Index];
                break;
            }
            case 2:
            {
                secondSide = MealManager.Instance.SecondSides[Index];
                break;
            }
        }
    }
}
