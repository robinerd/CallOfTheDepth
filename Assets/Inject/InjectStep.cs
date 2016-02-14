using UnityEngine;
using System.Collections;

public abstract class InjectStep
{
    public enum InjectStepType
    {
        OBJ,
        TAG,
        COMP
    }

    public InjectStepType type;
    public string name;

    public abstract UnityEngine.Object[] Evaluate(GameObject parent);

    public InjectStep(InjectStepType type, string name)
    {
        this.type = type;
        this.name = name;
    }
}