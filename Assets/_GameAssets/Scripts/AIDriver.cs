using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : Driver {

    private NueralNetwork brain;
    private CarMotor carMotor;
    private LayerMask trackMask;
    private Rigidbody2D carRb;

    public AIDriver(GameObject carGameO)
    {
        carGO = carGameO;

        //Create brain
        brain = new NueralNetwork(18, 5,5,5, 2);
        float[][] weights = brain.GetWeights();
        weights = randomizeWeights(weights);
        brain.SetWeights(weights);

        brain.SetWeights(brain.GetWeights());


        carRb = carGO.GetComponent<Rigidbody2D>();
        carMotor = carGO.GetComponent<CarMotor>();
        trackMask = carMotor.trackMask;
    }

    private float[][] randomizeWeights(float [][] weights)
    {

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                weights[i][j] = Random.Range(-1.0f, 1.0f);
            }
        }

        return weights;
    }

    float vertical = 0.0f;
    float horizontal = 0.0f;

    public float [][] GetDNA()
    {
        return brain.GetWeights();
    }

    public void SetDNA(float[][] dna)
    {
        brain.SetWeights(dna);
    }

    public float[] GetInputValues()
    {
        float[] networkInput = new float[18];
        float carAngle = carMotor.GetCarAngle();

        for (int i = 0; i < 16; i++)
        {
            float angle = (22.5f * i) + ((carAngle * Mathf.Rad2Deg));
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),Mathf.Sin(angle*Mathf.Deg2Rad));
            RaycastHit2D hit = Physics2D.Raycast(carGO.transform.position, direction, 1000.0f, trackMask);
            networkInput[i] = hit.distance;
        }

        networkInput[16] = carMotor.GetForwardVelocity().magnitude;
        networkInput[17] = carMotor.GetHorizontalVelocity().magnitude;
        //networkInput[17] = carAngle;

        return networkInput;
    }

    public override float GetHorizontal()
    {
        //Run the data through the nueral network
        float[] output = brain.RunNetwork(GetInputValues());
        vertical = output[0];
        horizontal = output[1];

        return horizontal;
    }

    public override float GetVertical()
    {
        return vertical;
    }

}
