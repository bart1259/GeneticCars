using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NueralNetwork {

    NueralNetworkLayer[] layers;

    public NueralNetwork(params int[] network)
    {
        layers = new NueralNetworkLayer[network.Length];
        for (int i = 0; i < network.Length; i++)
        {
            layers[i] = new NueralNetworkLayer(network[i]);
        }

        //Init all layers
        for (int i = 0; i < network.Length; i++)
        {
            NueralNetworkLayer prev = null;
            NueralNetworkLayer next = null;
            if (i != 0)
            {
                prev = layers[i - 1];
            }
            if (i != network.Length - 1)
            {
                next = layers[i + 1];
            }
            layers[i].Init(prev, next);
        }
    }

    //Get nueral networks weights
    public float [][] GetWeights()
    {
        float[][] weights = new float[layers.Length - 1][];
        for (int i = 1; i < layers.Length; i++)
        {
            float[] layerWeights = layers[i].GetWeights();
            weights[i-1] = layerWeights;
        }

        return weights;
    }

    //Set whole nueral networks weights
    public void SetWeights(float[][] weights)
    {
        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].SetWeights(weights[i-1]);
        }
    }

    public float[] RunNetwork(float[] input)
    {
        layers[0].SetAllValues(input);
        for (int i = 1; i < layers.Length; i++)
        {
            //string layerSummary = "";
            layers[i].Calculate();
            //for (int j = 0; j < layers[i].size; j++)
            {
                //layerSummary += layers[i].GetNueronValue(j) + " ";
            }
            //Debug.Log(layerSummary);
        }
        return layers[layers.Length - 1].GetAllValues();
    }

}

public class NueralNetworkLayer
{
    bool isInputLayer = false;
    bool isOutputLayer = false;

    NueralNetworkLayer previousLayer = null;
    NueralNetworkLayer nextLayer = null;

    public int size { get; protected set; }
    float[][] nueronWeights;
    float[] nueronBias;
    float[] nueronvalues;

    public NueralNetworkLayer(int size)
    {
        this.size = size;
        nueronBias = new float[size];
        nueronvalues = new float[size];
        nueronWeights = new float[size][];
    }

    public void Init(NueralNetworkLayer prev, NueralNetworkLayer next)
    {
        previousLayer = prev;
        nextLayer = next;

        if (previousLayer == null)
        {
            isInputLayer = true;
        }
        if(previousLayer == null)
        {
            isOutputLayer = true;
        }

        //If the layer is an input layer, weights are not needed
        if (isInputLayer)
        {
            return;
        }

        //Init weights array
        for (int i = 0; i < size; i++)
        {
            nueronWeights[i] = new float[previousLayer.size];
        }
    }

    public float[] GetWeights()
    {
        //Get size of weights array
        float[] weights = new float[((size) * previousLayer.size) + size];
        //Set values of weights
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < previousLayer.size; j++)
            {
                weights[(i * previousLayer.size) + j] = nueronWeights[i][j];
            }
        }
        //Set bias values
        for (int i = 0; i < size; i++)
        {
            weights[((size) * previousLayer.size) + i] = nueronBias[i];
        }
        return weights;
    }

    public void SetWeights(float[] weights)
    {
        //Set values of weights
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < previousLayer.size; j++)
            {
                nueronWeights[i][j] = weights[(i * previousLayer.size) + j];
            }
        }
        //Set bias values
        for (int i = 0; i < size; i++)
        {
            nueronBias[i] = weights[((size) * previousLayer.size) + i];
        }
    }

    public float GetNueronValue(int index)
    {
        return nueronvalues[index];
    }

    public float[] GetAllValues()
    {
        return nueronvalues;
    }

    public void SetAllValues(float[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            nueronvalues[i] = values[i];
        }
    }

    public void Calculate()
    {
        for (int i = 0; i < size; i++)
        {
            //Calculate individual nuerons value
            //weighted sum
            float weightedSum = 0.0f;
            for (int j = 0; j < nueronWeights[i].Length; j++)
            {
                weightedSum += previousLayer.GetNueronValue(j) * nueronWeights[i][j];
            }
            //add bias
            weightedSum += nueronBias[i];
            //final calculation
            nueronvalues[i] = ActivationFunction(weightedSum);
        }
    }

    //tanh
    private float ActivationFunction(float value)
    {
        return (2.0f / (1 + Mathf.Pow(2.71828f, -2.0f * value))) - 1.0f;
    }
}