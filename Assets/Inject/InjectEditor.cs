using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
public class InjectEditor : EditorWindow {

    [MenuItem("Inject/Inject Script Dependencies")]
    public static void Init()
    {
        InjectDependencies();
    }

    [ExecuteInEditMode]
    public void Awake()
    {
        EditorApplication.playmodeStateChanged += PlaymodeCallback;
    }

    public static void PlaymodeCallback()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.Log("PLAYING");
        }
        else
            Debug.Log("NOT PLAYING");

        //InjectDependencies();
    }

    public static void InjectDependencies()
    {
        // Get existing open window or if none, make a new one:
        foreach (BaseBehaviour obj in Resources.FindObjectsOfTypeAll<BaseBehaviour>())
        {
            obj.InjectDependencies(true, true);
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
#endif