using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateModule : BaseModule
{
    public UpdateModule()
    {
        PreFabs = "Perfabs/Panel/UpdatePanel";
    }


    public override Type GetView()
    {
        return typeof(UpdateView);
    }
}
