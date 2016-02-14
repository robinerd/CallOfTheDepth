using UnityEngine;
using System.Collections;

public class InjectStep_Tag : InjectStep
{
    public InjectStep_Tag(string name) : base(InjectStepType.TAG, name)
    { }

    public override UnityEngine.Object[] Evaluate(GameObject parent)
    {
        if (parent == null)
        {
            return GameObject.FindGameObjectsWithTag(name);
        }
        else
        {
            throw new System.NotSupportedException("Can only look for tags globally.");
        }
    }
}
