using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMotor : MonoBehaviour {

    public float forwardAcceleration;
    public float turnTorque;
    public float driftFactorSticky = 0.95f;
    public float driftFactorSlippy = 0.1f;
    public float maxStickyVelocity = 0.1f;
    public LayerMask trackMask;

    public enum DriverType
    {
        player,
        robot
    }
    public DriverType driverType;

    private List<Vector2> driftPoints;
    private bool drifinting = false;
    private float driftFactor = 0.75f;
    private Rigidbody2D rb;
    private Driver driver;

    //Checkpoints
    public int maxLaps;
    public int maxCheckpoints;
    private int lastCheckPoint = 0;
    private float lastCheckPointTime = 1000000.0f;
    private int laps = 0;
    private float timer = 0.0f;
    private bool done = false;

    //Collisions
    private int numberOfCollisions = 0;
    private float collisionTime = 0;
    private int lastCPNumCollisions = 0;
    private float lastCPCollisionTime = 0;

    // Use this for initialization
    void Awake () {
        driftPoints = new List<Vector2>();
        rb = GetComponent<Rigidbody2D>();

        switch (driverType)
        {
            case DriverType.player:
                driver = new PlayerDriver();
                break;
            case DriverType.robot:
                driver = new AIDriver(gameObject);
                break;
        }

        driver.carGO = gameObject;

        //Reset timer
        timer = 0.0f;
    }

	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;

        float vertical = driver.GetVertical();
        float horizontal = driver.GetHorizontal();

        if (vertical < 0 && Vector2.Dot( GetForwardVelocity(), transform.up) < 0.0f)
            vertical *= 0.2f;

        rb.AddForce(transform.up * vertical * forwardAcceleration);
        rb.AddTorque(-turnTorque * horizontal);

        driftFactor = driftFactorSticky;
        drifinting = false;
        if (GetHorizontalVelocity().magnitude > maxStickyVelocity)
        {
            driftFactor = driftFactorSlippy;
            drifinting = true;
        }

        if (drifinting)
        {
            driftPoints.Add(transform.position);
        }

        rb.velocity = GetForwardVelocity() + GetHorizontalVelocity() * driftFactor;
    }

    public void ResetCar()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        timer = 0;
        laps = 0;
        lastCheckPoint = 0;
        lastCheckPointTime = 10000000.0f;
        done = false;
        numberOfCollisions = 0;
        collisionTime = 0.0f;
        lastCPNumCollisions = 0;
        lastCPCollisionTime = 0;
    }    

    public float[][] GetDNA()
    {
        return ((AIDriver)driver).GetDNA();
    }

    public void SetDNA(float[][] dna)
    {
        ((AIDriver)driver).SetDNA(dna);
    }

    public float GetFitnessFunction()
    {
        int checkps = ((laps * (maxCheckpoints + 3)) + lastCheckPoint) * 1000;
        if (done)
            checkps = ((maxLaps + 1) * (maxCheckpoints + 3));
        float adjustedCheckps = checkps - Mathf.Pow((numberOfCollisions + collisionTime)*12f,1.5f);
        return Mathf.Pow(adjustedCheckps, 1.5f) / Mathf.Pow(lastCheckPointTime,0.25f);
    }

    //called when car gets check points
    public void GetCheckPoint(int checkpointID)
    {
        if (done)
            return;

        //Check to make sure car is going forward
        if (lastCheckPoint + 1 == checkpointID)
        {
            lastCheckPoint = checkpointID;
            lastCheckPointTime = timer;

            lastCPCollisionTime = collisionTime;
            lastCPNumCollisions = numberOfCollisions;
        }

        //check if car did lap
        else if (lastCheckPoint == maxCheckpoints && checkpointID == 0)
        {
            lastCheckPoint = 0;
            laps++;
            lastCheckPointTime = timer;
            if (laps == maxLaps)
            {
                done = true;
            }

            lastCPCollisionTime = collisionTime;
            lastCPNumCollisions = numberOfCollisions;
        }
    }

    //Get car angle
    public float GetCarAngle()
    {
        return Mathf.Atan2(transform.up.y, transform.up.x);
    }

    public Vector2 GetForwardVelocity()
    {
        return transform.up * Vector2.Dot(rb.velocity, transform.up);
    }

    public Vector2 GetHorizontalVelocity()
    {
        return transform.right * Vector2.Dot(rb.velocity, transform.right);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        numberOfCollisions++;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collisionTime += Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        //Drift debug
        bool disabled = true;
        if (driftPoints == null || disabled)
            return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < driftPoints.Count; i++)
        {
            Gizmos.DrawSphere(driftPoints[i], 0.1f);
        }
    }
}
