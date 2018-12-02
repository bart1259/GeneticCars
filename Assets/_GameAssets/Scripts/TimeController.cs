using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for controlling simulation speed
public class TimeController : MonoBehaviour {

    public Text speedMultiplerText;

    const float defaultFixedTime = 0.02f;
    const float defaultTimeScale = 1.0f;

    private Slider slider;

	void Start () {
        //Get slider
        slider = GetComponent<Slider>();
    }
	
	void Update () {
        int multiplier = Mathf.RoundToInt(slider.value);

        //Calculate actual speed
        float actualSpeedMultipler = Mathf.Pow(2, multiplier - 3);

        //Update text
        speedMultiplerText.text = actualSpeedMultipler + "x";

        //Update times
        Time.timeScale = actualSpeedMultipler * defaultTimeScale;
        Time.fixedDeltaTime = actualSpeedMultipler * defaultFixedTime;
    }
}
