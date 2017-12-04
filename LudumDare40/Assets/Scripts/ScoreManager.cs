using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    static ScoreManager instance;
    public static ScoreManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }

    public Text tips;

    int score = 0;
    public int Score
    {
        get { return score; }
        set
        {
            if (value > score)
            {
                audioSource.Play();
            }

            score = Mathf.Clamp(value, 0, 999);
            tips.text = "" + score;
        }
    }

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Instance = this;
    }
}
