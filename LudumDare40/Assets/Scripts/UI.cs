using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIState
{
    Uninitialized,
    Logo,
    Gameplay
}

public class UI : MonoBehaviour
{
    public GameObject logo;
    public GameObject score;

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
                        score.SetActive(false);
                        Time.timeScale = 0.0f;
                        break;
                    }
                    case UIState.Gameplay:
                    {
                        logo.SetActive(false);
                        score.SetActive(true);
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
                        State = UIState.Gameplay;
                }
                break;
            }
        }
    }
}
