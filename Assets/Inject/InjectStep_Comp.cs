using UnityEngine;
using System.Collections;
using System;

public class InjectStep_Comp : InjectStep
{
    Type compType;
    public InjectStep_Comp(string name, Type compType) : base(InjectStepType.COMP, name)
    {
        this.compType = compType;
    }

    public override UnityEngine.Object[] Evaluate(GameObject parent)
    {
        if (parent == null)
        {
            return GameObject.FindObjectsOfType(compType);
        }
        else
        {
            ArrayList result = new ArrayList();
            result.AddRange(parent.GetComponents(compType));
            //result.AddRange(parent.GetComponents(compType));

            return (UnityEngine.Object[])result.ToArray(compType);
        }
    }
}
