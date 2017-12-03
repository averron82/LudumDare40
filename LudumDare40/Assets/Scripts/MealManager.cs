using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ingredient
{
    public Sprite sprite;
};

[System.Serializable]
public class Meal
{
    public Ingredient Main;
    public Ingredient FirstSide;
    public Ingredient SecondSide;

    public int NumCommonIngredients(Meal other)
    {
        int result = 0;

        if (Main == other.Main)
        {
            ++result;
        }

        if (FirstSide == other.FirstSide)
        {
            ++result;
        }

        if (SecondSide == other.SecondSide)
        {
            ++result;
        }

        return result;
    }
};

public class MealManager : MonoBehaviour
{
    static MealManager instance;
    public static MealManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    public List<Ingredient> Mains = new List<Ingredient>();
    public List<Ingredient> FirstSides = new List<Ingredient>();
    public List<Ingredient> SecondSides = new List<Ingredient>();

    public Meal GenerateMeal()
    {
        Meal result = new Meal();

        if (Mains.Count > 0)
        {
            result.Main = Mains[Random.Range(0, Mains.Count)];
        }

        if (FirstSides.Count > 0)
        {
            result.FirstSide = FirstSides[Random.Range(0, FirstSides.Count)];
        }

        if (SecondSides.Count > 0)
        {
            result.SecondSide = SecondSides[Random.Range(0, SecondSides.Count)];
        }

        return result;
    }

    void Start()
    {
        Instance = this;
    }
}
