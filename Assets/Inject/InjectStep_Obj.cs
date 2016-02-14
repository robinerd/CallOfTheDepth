using UnityEngine;
using System.Collections;

public class InjectStep_Obj : InjectStep
{
    public InjectStep_Obj(string name) : base(InjectStepType.OBJ, name)
    { }

    public override UnityEngine.Object[] Evaluate(GameObject parent)
    {
        if (parent == null)
        {
            GameObject obj = GameObject.Find(name);
            if (!obj)
                return null;
            return new UnityEngine.Object[] { obj };
        }
        else
        {
            Transform obj = parent.transform.Find(name);
            if (!obj)
                return null;
            return new UnityEngine.Object[] { obj.gameObject };
        }
    }
}