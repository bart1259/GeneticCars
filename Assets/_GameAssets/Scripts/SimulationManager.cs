using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SimulationManager : MonoBehaviour {

    public string pathToLoadBrainsFrom;
    public int numberOfCars;
    public float maxTime;
    public float resetCheckInterval;
    public GameObject simCarPrefab;
    public Transform startLocation;
    public float mutationRate;
    public float mutationAmount;
    public float purgeRate;
    public List<float[][]> lastRoundDNA { get; set; }
    public int generationNumber = 1;

    private CarMotor[] simCars;
    private float[] lastTimeFitnessFunction;
    private float timer;
    private float totalTimeTimer;

	// Use this for initialization
	void Start () {

        lastRoundDNA = new List<float[][]>();
        timer = resetCheckInterval;
        simCars = new CarMotor[numberOfCars];
        lastTimeFitnessFunction = new float[numberOfCars];

        //Load brains
        List<float[][]> loadedBrains = new List<float[][]>();
        //Check if file exists
        if(pathToLoadBrainsFrom != null && File.Exists(pathToLoadBrainsFrom))
        {
            //Open file for reading
            StreamReader reader = new StreamReader(pathToLoadBrainsFrom);
            string numberOfBrains = reader.ReadLine();
            int numberOfBrainsToLoad = int.Parse(numberOfBrains.TrimEnd(';'));
            for (int i = 0; i < numberOfBrainsToLoad; i++)
            {
                //Make brains dna
                float[][] brainDNA;

                string structure = reader.ReadLine();
                string[] structureArray = structure.Split(';');
                int dnaMajorLength = int.Parse(structureArray[0]);
                brainDNA = new float[dnaMajorLength][];

                for (int j = 1; j < dnaMajorLength + 1; j++)
                {
                    int dnaMinorLength = int.Parse(structureArray[j]);
                    brainDNA[j - 1] = new float[dnaMinorLength];

                    string genes = reader.ReadLine();
                    string[] genesArray = genes.Split(';');
                    for (int k = 0; k < genesArray.Length; k++)
                    {
                        brainDNA[j-1][k] = float.Parse(genesArray[k]);
                    }
                }

                loadedBrains.Add(brainDNA);
            }
        }

        for (int i = 0; i < numberOfCars; i++)
        {
            GameObject simCarInstance;
            if (loadedBrains.Count > i)
            {
                simCarInstance = CreateNewCar(loadedBrains[i], Color.black);
            }
            else
            {
                simCarInstance = CreateNewCar(null, Color.black);
            }
            simCarInstance.name = "Car Sim: " + i;
            simCars[i] = simCarInstance.GetComponent<CarMotor>();
        }
	}

    private void Update()
    {
        timer -= Time.deltaTime;


        if (timer <= 0)
        {
            timer = resetCheckInterval;

            if (CheckReset())
            {
                ResetSim();
            }
        }

        totalTimeTimer += Time.deltaTime;
        if (totalTimeTimer >= maxTime)
        {
            ResetSim();
        }
    }

    private GameObject CreateNewCar(float[][] dna, Color color)
    {
        GameObject simCarInstance = Instantiate(simCarPrefab, startLocation.position, startLocation.rotation);
        simCarInstance.transform.SetParent(transform);
        if (color != Color.black)
            simCarInstance.GetComponent<SpriteRenderer>().color = new Color(Mathf.Clamp01( Random.Range(-0.1f, 0.1f) + color.r), Mathf.Clamp01(Random.Range(-0.1f, 0.1f) + color.g), Mathf.Clamp01(Random.Range(-0.1f, 0.1f) + color.b));
        else
            simCarInstance.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        if (dna != null)
        {
            simCarInstance.GetComponent<CarMotor>().SetDNA(dna);
        }
        return simCarInstance;
    }

    void ResetSim()
    {
        //Reset timers
        timer = resetCheckInterval;
        totalTimeTimer = 0.0f;

        //Sort by car fitness
        List<CarMotor> sortedCars = simCars.OrderBy(o => o.GetFitnessFunction()).ToList();

        //Save the dna
        lastRoundDNA.Clear();
        for (int i = 0; i < sortedCars.Count; i++)
        {
            lastRoundDNA.Add(sortedCars[i].GetDNA());
        }

        //Destroy worst
        for (int i = Mathf.RoundToInt(numberOfCars * purgeRate); i >= 0; i--)
        {
            DestroyImmediate(sortedCars[i].gameObject);
        }

        List<CarMotor> carsLeft = new List<CarMotor>();
        for (int i = 0; i < simCars.Length; i++)
        {
            if (simCars[i] != null)
            {
                carsLeft.Add(simCars[i]);
            }
        }
        
        int index = 0;
        while (carsLeft.Count != numberOfCars)
        {
            carsLeft.Add(CreateNewCar(mutateDNA(carsLeft[index].GetDNA()), carsLeft[index].GetComponent<SpriteRenderer>().color).GetComponent<CarMotor>());
            index++;
        }

        Debug.Log("Gen: " + generationNumber + " Best Fitness = " + sortedCars[sortedCars.Count - 1].GetFitnessFunction() + " Col: r:" + sortedCars[sortedCars.Count - 1].GetComponent<SpriteRenderer>().color.r + " g: " + sortedCars[sortedCars.Count - 1].GetComponent<SpriteRenderer>().color.g + " b: " + sortedCars[sortedCars.Count - 1].GetComponent<SpriteRenderer>().color.b);

        simCars = carsLeft.ToArray();
        for (int i = 0; i < simCars.Length; i++)
        {
            simCars[i].gameObject.name = "Sim car: " + i;
            simCars[i].transform.position = startLocation.position;
            simCars[i].transform.rotation = startLocation.rotation;
            simCars[i].ResetCar();
        }
        lastTimeFitnessFunction = new float[numberOfCars];
        generationNumber++;
    }

    float[][] mutateDNA(float[][] dna) {

        for (int i = 0; i < dna.Length; i++)
        {
            for (int j = 0; j < dna[i].Length; j++)
            {
                if (Random.Range(0.0f,1.0f) < mutationRate)
                {
                    dna[i][j] = dna[i][j] + Random.Range(-mutationAmount, mutationAmount);
                }
            }
        }

        return dna;
    }

    bool CheckReset()
    {
        bool reset = true;

        for (int i = 0; i < numberOfCars; i++)
        {
            float fintessNow = simCars[i].GetFitnessFunction();
            if (fintessNow > lastTimeFitnessFunction[i])
            {
                lastTimeFitnessFunction[i] = fintessNow;
                reset = false;
            }
        }

        return reset;
    }
}
