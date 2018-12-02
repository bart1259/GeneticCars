﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDriver : Driver {

    public override float GetHorizontal()
    {
        return Input.GetAxis("Horizontal");
    }

    public override float GetVertical()
    {
        return Input.GetAxis("Vertical");
    }
}
