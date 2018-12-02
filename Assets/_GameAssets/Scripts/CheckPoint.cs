using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public int checkPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<CarMotor>().GetCheckPoint(checkPoint);
    }
}
