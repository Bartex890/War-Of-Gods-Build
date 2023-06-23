using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class SimpleEditorUtils
{
    [MenuItem("Edit/Play-Unplay, But From Prelaunch Scene %F1")]
    public static void PlayFromPrelaunchScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Edit/Go to the main scene %F2")]
    public static void GoToMainScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            return;
        }
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/TestScenes/MapSelectionTest.unity");
    }
}