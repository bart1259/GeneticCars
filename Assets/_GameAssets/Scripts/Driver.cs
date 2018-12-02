using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Driver {

    public GameObject carGO;

    public abstract float GetVertical();
    public abstract float GetHorizontal();
}
