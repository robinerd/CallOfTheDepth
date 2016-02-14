using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
public class InjectEditor : EditorWindow {

    [MenuItem("Inject/Inject Script Dependencies")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        foreach(BaseBehaviour obj in GameObject.FindObjectsOfType<BaseBehaviour>())
        {
            obj.InjectDependencies(true, true);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
#endif