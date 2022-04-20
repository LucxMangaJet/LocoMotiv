using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

#if UNITY_EDITOR
[InitializeOnLoad]
public class InfoPopup
{
    static InfoPopup()
    {
        var assets = AssetDatabase.FindAssets("t:TextAsset", new string[] { "Assets/Popups" });
        EditorCoroutineUtility.StartCoroutineOwnerless(ShowPopupsRoutine(assets));
    }

    private static IEnumerator ShowPopupsRoutine(string[] assets)
    {
        yield return new WaitForSecondsRealtime(1);

        foreach (var item in assets)
        {
            var key = $"infoPopup_{item}";

            if (!EditorPrefs.HasKey(key))
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(item));

                if (asset != null)
                {
                    bool result = EditorUtility.DisplayDialog(asset.name, asset.text, "Don't show again", "Continue");

                    if (result)
                        EditorPrefs.SetBool(key, true);
                }
            }

            yield return null;
        }
    }
}
#endif