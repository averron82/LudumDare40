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
    public static int NumInState(CustomerState state)
    {
        int result = 0;

        Customer[] customers = FindObjectsOfType<Customer>();
        foreach (Customer customer in customers)
        {
            if (customer.State == state)
            {
                ++result;
            }
        }

        return result;
    }

    public const float DEFAULT_MOODLET_TIME = 2.0f;
    public const float DEFAULT_START_FOLLOW_DISTANCE = 0.6f;
    public const float DEFAULT_STOP_FOLLOW_DISTANCE = 0.5f;

    public float MoodAdjustWaitingToBeSeated = -1f;
    public float MoodAdjustFollowingWaiterToTable = -1f;
    public float MoodAdjustConsideringOrder = 0.0f;
    public float MoodAdjustWaitingToPlaceOrder = -1f;
    public float MoodAdjustWaitingForMeal = -1f;
    public float MoodAdjustEatingMeal = 0.0f;

    public float MoveSpeed = 300.0f;

    public Transform MoveTarget;
    public Table table;
    public Customer PlusOne;

    public float StartFollowDistance = DEFAULT_START_FOLLOW_DISTANCE;
    public float StopFollowDistance = DEFAULT_STOP_FOLLOW_DISTANCE;

    public OrderBubble orderBubble;
    public Moodlet moodlet;

    float mood = 100.0f;
    public float Mood
    {
        get
        {
            return mood;
        }

        set
        {
            mood = Mathf.Clamp(value, 0.0f, 100.0f);

            if (PlusOne)
            {
                PlusOne.Mood = value;
            }

            if (mood == 0.0f && State != CustomerState.Leaving && State != CustomerState.PlusOne)
            {
                moodlet.ShowForSeconds(MoodletType.Angry, DEFAULT_MOODLET_TIME);
                if (State == CustomerState.WaitingToBeSeated)
                {
                    QueueManager.Instance.RemoveCustomer(this);
                }
                State = CustomerState.Leaving;
            }
        }
    }

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
                    orderBubble.Show(desiredMeal, gameObject.transform.position.x > 0.5f);
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
                    StartCoroutine(EatMeal(Random.Range(8.0f, 10.0f)));
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
        if (State == CustomerState.Uninitialized)
        {
            State = CustomerState.WaitingToBeSeated;
            if (Random.Range(0.0f, 1.0f) > 0.5f)
            {
                SpawnPlusOne();
            }
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
        PlusOne = CustomerFactory.Instance.CreateCustomer(transform.position);
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

        table.IncrementFoodBeingConsumed();

        switch (meal.NumCommonIngredients(desiredMeal))
        {
            case 3:
            {
                Mood += 10.0f;
                moodlet.ShowForSeconds(MoodletType.Happy, DEFAULT_MOODLET_TIME);
                break;
            }
            case 2:
            {
                Mood -= 10.0f;
                if (Mood > 0)
                {
                    moodlet.ShowForSeconds(MoodletType.Unhappy, DEFAULT_MOODLET_TIME);
                }
                break;
            }
            default:
            {
                Mood -= 20.0f;
                if (Mood > 0)
                {
                    moodlet.ShowForSeconds(MoodletType.Angry, DEFAULT_MOODLET_TIME);
                }
                break;
            }
        }

        if (State == CustomerState.WaitingForMeal)
        {
            State = CustomerState.EatingMeal;
        }
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
            if (ShouldMove)
            {
                ReachTarget();
                ShouldMove = false;
            }
        }

        if (ShouldMove)
        {
            Vector2 force = new Vector2(ToMoveTarget.x, ToMoveTarget.y);

            // Avoid obstacles.
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

            GameObject nearestObstacle = null;
            float nearestDistSq = float.MaxValue;
            foreach (GameObject candidate in obstacles)
            {
                Vector3 toCandidate = candidate.transform.position - transform.position;
                float distSq = toCandidate.sqrMagnitude;
                if (distSq < nearestDistSq)
                {
                    nearestObstacle = candidate;
                    nearestDistSq = distSq;
                }
            }

            if (nearestObstacle)
            {
                Vector3 obstaclePosition = nearestObstacle.transform.position;
                Vector3 toObstacle = obstaclePosition - Position;
                if (toObstacle.sqrMagnitude < ToMoveTarget.sqrMagnitude)
                {
                    force -= new Vector2(toObstacle.x, toObstacle.y).normalized * 0.4f;
                }
            }

            force = force.normalized * MoveSpeed * Time.deltaTime;
            rigidBody.AddForce(force);
        }
    }

    void ReachTarget()
    {
        switch (state)
        {
            case CustomerState.Leaving:
            {
                ScoreManager.Instance.Score += (int)(Mood / 10);
                if (PlusOne)
                {
                    PlusOne.State = CustomerState.Leaving;
                }
                Destroy(gameObject);
                break;
            }
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

    IEnumerator ShowFollowMe(Waiter waiter, float Seconds)
    {
        yield return new WaitForSeconds(Seconds);

        waiter.FollowMeBubble.SetActive(false);
    }

    void Leave()
    {
        GameObject Exit = GameObject.FindGameObjectWithTag("Exit");
        MoveTarget = Exit.transform;
        StartFollowDistance = DEFAULT_START_FOLLOW_DISTANCE;
        StopFollowDistance = DEFAULT_STOP_FOLLOW_DISTANCE;

        if (table)
        {
            table.SetAvailable();
            table = null;
        }

        if (PlusOne)
        {
            PlusOne.MoveTarget = transform;
            PlusOne.StartFollowDistance = DEFAULT_START_FOLLOW_DISTANCE;
            PlusOne.StopFollowDistance = DEFAULT_STOP_FOLLOW_DISTANCE;
            PlusOne.table = null;
        }
    }
}
