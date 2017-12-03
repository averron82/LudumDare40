using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoodletType
{
    Happy,
    Unhappy,
    Angry,
    Impatient,
    RequiresAttention
}

public class Moodlet : MonoBehaviour
{
    public SpriteRenderer icon;

    public Sprite happy;
    public Sprite unhappy;
    public Sprite angry;
    public Sprite impatient;
    public Sprite requiresAttention;

    bool visible;

    public void Show(MoodletType type)
    {
        switch (type)
        {
            case MoodletType.Happy:
            {
                icon.sprite = happy;
                break;
            }
            case MoodletType.Unhappy:
            {
                icon.sprite = unhappy;
                break;
            }
            case MoodletType.Angry:
            {
                icon.sprite = angry;
                break;
            }
            case MoodletType.Impatient:
            {
                icon.sprite = impatient;
                break;
            }
            case MoodletType.RequiresAttention:
            {
                icon.sprite = requiresAttention;
                break;
            }
        }

        gameObject.SetActive(true);
        visible = true;
    }

    public void ShowForSeconds(MoodletType type, float seconds)
    {
        Show(type);
        StartCoroutine(HideInSeconds(seconds));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        visible = false;
    }

    public IEnumerator HideInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (IsVisible())
        {
            Hide();
        }
    }

    public bool IsVisible()
    {
        return visible;
    }
}
