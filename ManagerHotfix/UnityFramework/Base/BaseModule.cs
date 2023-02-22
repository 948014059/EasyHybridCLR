using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseModule 
{
    public enum LayerType
    {
        UI,
        Tips,
        Log,
    }

    public string PreFabs = "";
    public LayerType layer = LayerType.UI;

    public abstract Type GetView();
}
