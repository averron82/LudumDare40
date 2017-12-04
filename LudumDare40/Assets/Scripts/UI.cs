using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    Uninitialized,
    Logo,
    Instructions,
    Gameplay
}

public class UI : MonoBehaviour
{
    public GameObject logo;
    public GameObject instructions;
    public GameObject score;
    public GameObject fade;

    public float instructionTime = 2.0f;

    UIState state = UIState.Uninitialized;

    public UIState State
    {
        get { return state; }

        set
        {
            if (state != value)
            {
                state = value;

                switch (state)
                {
                    case UIState.Logo:
                    {
                        logo.SetActive(true);
                        instructions.SetActive(false);
                        score.SetActive(false);
                        fade.SetActive(true);
                        Time.timeScale = 0.0f;
                        break;
                    }
                    case UIState.Instructions:
                    {
                        logo.SetActive(false);
                        instructions.SetActive(true);
                        score.SetActive(false);
                        fade.SetActive(true);
                        Time.timeScale = 0.0f;
                        StartCoroutine(HideInstructions(instructionTime));
                        break;
                    }
                    case UIState.Gameplay:
                    {
                        logo.SetActive(false);
                        instructions.SetActive(false);
                        score.SetActive(true);
                        fade.SetActive(false);
                        Time.timeScale = 1.0f;
                        break;
                    }
                }
            }
        }
    }

    void Start()
    {
        State = UIState.Logo;
    }

    void Update()
    {
        switch (state)
        {
            case UIState.Logo:
            {
                if (Input.anyKeyDown)
                {
                    State = UIState.Instructions;
                }
                break;
            }
            case UIState.Instructions:
            {
                if (Input.anyKeyDown)
                {
                    State = UIState.Gameplay;
                }
                break;
            }
        }
    }

    IEnumerator HideInstructions(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        if (State == UIState.Instructions)
        {
            State = UIState.Gameplay;
        }
    }
}
