using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour {

    public Text generationText;
    public Text simulationLengthText;
    public Slider simulationLengthSlider;
    public float maxSimLength = 360;

    private SimulationManager simulationManager;

	void Start () {
        simulationManager = GetComponent<SimulationManager>();
    }
	
	void Update () {
        generationText.text = "Generation: " + simulationManager.generationNumber;
        simulationLengthText.text = (360 * simulationLengthSlider.value) + "s";
        simulationManager.maxTime = (360 * simulationLengthSlider.value);
    }
}
