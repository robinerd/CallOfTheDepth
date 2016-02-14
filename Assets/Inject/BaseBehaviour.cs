using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class BaseBehaviour : MonoBehaviour {

    const BindingFlags FIELD_FLAGS = 
        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

    /*
    virtual protected void Awake()
    {
        InjectDependencies(true, true);
    }*/

    public void InjectDependencies(bool forceReset, bool logErrors)
    {
        foreach (FieldInfo field in this.GetType().GetFields(FIELD_FLAGS))
        {
            foreach (Attribute attr in field.GetCustomAttributes(true)) {
                if (attr is Inject)
                {
                    if (field.GetValue(this) == null || forceReset)
                    {
                        try
                        {
                            Type targetType = field.FieldType.GetElementType(); //Used to get type in the array elements
                            if (targetType == null) //will be null if not an array.
                                targetType = field.FieldType;

                            UnityEngine.Object[] results = ((Inject)attr).Evaluate(targetType);
                            if (results == null || results.Length == 0)
                            {
                                if (logErrors)
                                    Debug.LogError("Missing Inject '" + ((Inject)attr).Query + "' at:\n" + 
                                                   " - Object '" + this.gameObject.name + "'\n" +
                                                   " - Script '" + this.GetType().Name + "'\n" +
                                                   " - Variable '" + field.FieldType.Name + " " + field.Name + "'");
                                field.SetValue(this, null);
                            }
                            else
                            {
                                if (field.FieldType.IsArray)
                                    field.SetValue(this, results);
                                else
                                    field.SetValue(this, results[0]);
                            }
                        }
                        catch(Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                }
            }
        }
    }

}
