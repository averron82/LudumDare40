using System.Collections;
using UnityEngine;

public enum CustomerState
{
    Uninitialized,

    WaitingToBeSeated,
    FollowingWaiterToTable,
    ConsideringOrder,
    WaitingToPlaceOrder,
    PlacingOrder,
    WaitingForMeal,
    EatingMeal,
    Leaving,

    PlusOne
};

public class Customer : MonoBehaviour
{
    public const float DEFAULT_MOODLET_TIME = 2.0f;
    public const float DEFAULT_START_FOLLOW_DISTANCE = 1.0f;
    public const float DEFAULT_STOP_FOLLOW_DISTANCE = 0.5f;

    public float MoodAdjustWaitingToBeSeated = -0.1f;
    public float MoodAdjustFollowingWaiterToTable = -0.1f;
    public float MoodAdjustConsideringOrder = 0.0f;
    public float MoodAdjustWaitingToPlaceOrder = -0.1f;
    public float MoodAdjustWaitingForMeal = -0.1f;
    public float MoodAdjustEatingMeal = 0.0f;

    public float MoveSpeed = 300.0f;

    public Transform MoveTarget;
    public Table table;
    public Customer PlusOne;

    public float StartFollowDistance = DEFAULT_START_FOLLOW_DISTANCE;
    public float StopFollowDistance = DEFAULT_STOP_FOLLOW_DISTANCE;

    public OrderBubble orderBubble;
    public Moodlet moodlet;

    float Mood = 100.0f;

    bool ShouldMove = false;

    Interactive interactive;

    Meal desiredMeal;

    Rigidbody2D rigidBody;

    CustomerState state = CustomerState.Uninitialized;
    public CustomerState State
    {
        get { return state; }

        set
        {
            if (state == value)
            {
                return;
            }

            state = value;
            switch (state)
            {
                case CustomerState.WaitingToBeSeated:
                {
                    StartCoroutine(BecomeBoredOfWaiting(state, Random.Range(15.0f, 20.0f)));
                    break;
                }
                case CustomerState.ConsideringOrder:
                {
                    StartCoroutine(WaitToPlaceOrder(Random.Range(5.0f, 10.0f)));
                    break;
                }
                case CustomerState.WaitingToPlaceOrder:
                {
                    moodlet.Show(MoodletType.RequiresAttention);
                    break;
                }
                case CustomerState.PlacingOrder:
                {
                    moodlet.Hide();
                    orderBubble.Show(desiredMeal);
                    StartCoroutine(WaitForMeal(3.0f));
                    break;
                }
                case CustomerState.WaitingForMeal:
                {
                    orderBubble.Hide();
                    StartCoroutine(BecomeBoredOfWaiting(state, Random.Range(15.0f, 20.0f)));
                    break;
                }
                case CustomerState.EatingMeal:
                {
                    StartCoroutine(EatMeal(Random.Range(10.0f, 12.0f)));
                    break;
                }
                case CustomerState.Leaving:
                {
                    Leave();
                    break;
                }
            }
        }
    }

    void Start()
    {
        if ((state != CustomerState.PlusOne) && Random.Range(0.0f, 1.0f) > 0.5f)
        {
            SpawnPlusOne();
        }

        interactive = GetComponent<Interactive>();
        interactive.SetInteraction(Interact, ValidateInteraction);

        if (MealManager.Instance)
        {
            desiredMeal = MealManager.Instance.GenerateMeal();
        }

        rigidBody = GetComponent<Rigidbody2D>();
        if (!rigidBody)
        {
            Debug.LogError(
                string.Format("Failed to retrieve RigidBody2D on {0}", gameObject.name));
        }
    }

    void SpawnPlusOne()
    {
        PlusOne = Instantiate(this, transform.position, Quaternion.identity);
        PlusOne.MoveTarget = transform;
        PlusOne.state = CustomerState.PlusOne;
    }

    void Interact(Waiter waiter)
    {
        switch (state)
        {
            case CustomerState.WaitingToPlaceOrder:
            {
                State = CustomerState.PlacingOrder;
                break;
            }
            case CustomerState.WaitingForMeal:
            {
                TakeMeal(waiter);
                break;
            }
        }
    }

    void TakeMeal(Waiter waiter)
    {
        Meal meal = waiter.Meal;
        waiter.Meal = null;

        switch (meal.NumCommonIngredients(desiredMeal))
        {
            case 3:
            {
                moodlet.ShowForSeconds(MoodletType.Happy, DEFAULT_MOODLET_TIME);
                break;
            }
            case 2:
            {
                moodlet.ShowForSeconds(MoodletType.Unhappy, DEFAULT_MOODLET_TIME);
                break;
            }
            default:
            {
                moodlet.ShowForSeconds(MoodletType.Angry, DEFAULT_MOODLET_TIME);
                break;
            }
        }

        State = CustomerState.EatingMeal;
    }

    bool ValidateInteraction(Waiter waiter)
    {
        if (waiter.Follower)
        {
            return false;
        }

        switch (State)
        {
            case CustomerState.WaitingToPlaceOrder:
            {
                return true;
            }
            case CustomerState.WaitingForMeal:
            {
                return waiter.Meal != null;
            }
        }

        return false;
    }

    void Update()
    {
        UpdateMood();
    }

    void UpdateMood()
    {
        switch (state)
        {
            case CustomerState.WaitingToBeSeated:
            {
                Mood += MoodAdjustWaitingToBeSeated * Time.deltaTime;
                break;
            }
            case CustomerState.FollowingWaiterToTable:
            {
                Mood += MoodAdjustFollowingWaiterToTable * Time.deltaTime;
                break;
            }
            case CustomerState.ConsideringOrder:
            {
                Mood += MoodAdjustConsideringOrder * Time.deltaTime;
                break;
            }
            case CustomerState.WaitingToPlaceOrder:
            {
                Mood += MoodAdjustWaitingToPlaceOrder * Time.deltaTime;
                break;
            }
            case CustomerState.WaitingForMeal:
            {
                Mood += MoodAdjustWaitingForMeal * Time.deltaTime;
                break;
            }
            case CustomerState.EatingMeal:
            {
                Mood += MoodAdjustEatingMeal * Time.deltaTime;
                break;
            }
        }

        Mood = Mathf.Clamp(Mood, 0.0f, 100.0f);
    }

    void FixedUpdate()
    {
        if (MoveTarget)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        Vector3 Position = transform.position;
        Vector3 MoveTargetPosition = MoveTarget.position;
        Vector3 ToMoveTarget = MoveTargetPosition - Position;
        float DistanceSq = ToMoveTarget.sqrMagnitude;

        if (DistanceSq >= (StartFollowDistance * StartFollowDistance))
        {
            ShouldMove = true;
        }
        else if (DistanceSq <= (StopFollowDistance * StopFollowDistance))
        {
            ShouldMove = false;
        }

        if (ShouldMove)
        {
            Vector2 force = new Vector2(ToMoveTarget.x, ToMoveTarget.y);

            // Avoid table.
            if (table)
            {
                Vector3 tablePosition = table.transform.position;
                Vector3 toTable = tablePosition - Position;
                if (toTable.sqrMagnitude < ToMoveTarget.sqrMagnitude)
                {
                    force -= new Vector2(toTable.x, toTable.y).normalized * 0.45f;
                }
            }

            force = force.normalized * MoveSpeed * Time.deltaTime;
            rigidBody.AddForce(force);
        }
    }

    IEnumerator BecomeBoredOfWaiting(CustomerState boredOfState, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (boredOfState == State)
        {
            if (Mood > 50.0f)
            {
                if (Random.Range(0, 2) == 0)
                {
                    moodlet.ShowForSeconds(MoodletType.Unhappy, DEFAULT_MOODLET_TIME);
                }
                else
                {
                    moodlet.ShowForSeconds(MoodletType.Impatient, DEFAULT_MOODLET_TIME);
                }
            }
            else
            {
                moodlet.ShowForSeconds(MoodletType.Angry, DEFAULT_MOODLET_TIME);
            }

            StartCoroutine(BecomeBoredOfWaiting(boredOfState, Random.Range(10.0f, 15.0f)));
        }
    }

    IEnumerator WaitToPlaceOrder(float Seconds)
    {
        yield return new WaitForSeconds(Seconds);

        State = CustomerState.WaitingToPlaceOrder;
    }

    IEnumerator WaitForMeal(float Seconds)
    {
        yield return new WaitForSeconds(Seconds);

        State = CustomerState.WaitingForMeal;
    }

    IEnumerator EatMeal(float Seconds)
    {
        yield return new WaitForSeconds(Seconds);

        State = CustomerState.Leaving;
    }

    void Leave()
    {
        GameObject Exit = GameObject.FindGameObjectWithTag("Exit");
        MoveTarget = Exit.transform;
        StartFollowDistance = DEFAULT_START_FOLLOW_DISTANCE;
        StopFollowDistance = DEFAULT_STOP_FOLLOW_DISTANCE;
        table.SetAvailable();
        table = null;

        if (PlusOne)
        {
            PlusOne.MoveTarget = transform;
            PlusOne.StartFollowDistance = DEFAULT_START_FOLLOW_DISTANCE;
            PlusOne.StopFollowDistance = DEFAULT_STOP_FOLLOW_DISTANCE;
            PlusOne.table = null;
        }
    }
}
