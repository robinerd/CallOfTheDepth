using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Field)]
public class Inject : Attribute
{
    List<InjectStep> steps;
    public string Query { get; private set; }

    public Inject(string query)
    {
        this.Query = query;
    }

    private void InitSteps(Type targetType)
    {
        steps = new List<InjectStep>();
        foreach (string queryPart in Query.Split(new char[] { '/' }))
        {
            if (queryPart.Length == 0)
            {
                continue;
            }
            switch (queryPart[0])
            {
                case '#':
                    steps.Add(new InjectStep_Tag(queryPart.Substring(1)));
                    break;
                /*case '.':
                    steps.Add(new InjectStep_Comp(queryPart.Substring(1)));
                    break;*/
                default:
                    steps.Add(new InjectStep_Obj(queryPart));
                    break;
            }
        }

        //Anything else than object is assumed to be a component
        if(targetType != typeof(GameObject))
        {
            steps.Add(new InjectStep_Comp(targetType.Name, targetType));
        }
    }

    public UnityEngine.Object[] Evaluate(Type targetType)
    {
        InitSteps(targetType);
        UnityEngine.Object[] prevResults = new UnityEngine.Object[] { null };

        foreach (InjectStep step in steps)
        {
            ArrayList results = new ArrayList();
            foreach (UnityEngine.Object parent in prevResults)
            {
                if (parent == null || parent.GetType() == typeof(GameObject))
                {
                    UnityEngine.Object[] partResults = step.Evaluate(parent as GameObject);
                    if(partResults != null && partResults.Length > 0)
                    {
                        foreach (UnityEngine.Object obj in partResults)
                            results.Add(obj);
                    }
                }
                else
                {
                    throw new NotSupportedException("Selecting anything except for game objects must be the last step of the query");
                }
            }
            if(results.Count == 0)
            {
                return null;
            }
            prevResults = (UnityEngine.Object[]) results.ToArray(results[0].GetType());
        }

        return prevResults;
    }

}