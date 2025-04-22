using System.IO;
using System.IO.Compression;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEditor.SceneManagement;

namespace StorytellerStudio.Editor
{
    public class AssetInfo
    {
        public string dbname;
        public string displayName;
        public string type;
    }

    [InitializeOnLoad]
    public static class ExporterSceneInstaller
    {
    private static readonly (string source, string target)[] scenePairs =
    {
        (
            "Packages/com.storytellerstudio.export-tools/Runtime/Asset Exporter/Asset Exporter.unity",
            "Assets/3DStoryteller/Scenes/Asset Exporter.unity"
        ),
        (
            "Packages/com.storytellerstudio.export-tools/Runtime/Asset Exporter/Asset Exporter Environment.unity",
            "Assets/3DStoryteller/Scenes/Asset Exporter Environment.unity"
        ),
        (
            "Packages/com.storytellerstudio.export-tools/Prefabs/Storyteller Studio.prefab",
            "Assets/3DStoryteller/Prefabs/Storyteller Studio.prefab"
        )
    };

    static ExporterSceneInstaller()
    {
        if (!SessionState.GetBool("ExporterScenesCopied", false))
        {
            foreach (var (source, target) in scenePairs)
            {
                CopyAssetIfNeeded(source, target);
            }

            SessionState.SetBool("ExporterScenesCopied", true);
        }
    }

    private static void CopyAssetIfNeeded(string sourcePath, string targetPath)
    {
    if (!File.Exists(sourcePath))
    {
        return;
    }

    if (File.Exists(targetPath))
    {
        // Already copied
        return;
    }

    string targetDir = Path.GetDirectoryName(targetPath);
    if (!Directory.Exists(targetDir))
    {
        Directory.CreateDirectory(targetDir);
    }

    File.Copy(sourcePath, targetPath);
    AssetDatabase.Refresh();
    }
    }
    
    [InitializeOnLoad]
    public class ExportAsset
    {
        private const string propExportScenePath = "Assets/3DStoryteller/Scenes/Asset Exporter.unity";
        private const string envExportScenePath = "Assets/3DStoryteller/Scenes/Asset Exporter Enviornment.unity";
        static ExportAsset()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                //EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

                if (SessionState.GetBool("ThumbnailCaptureCompleted", false))
                {
                    SessionState.SetBool("ThumbnailCaptureCompleted", false);
                    FinalizeAssetPackaging();
                }
            }
        }

        [MenuItem("Assets/3DStoryteller Studio/Export Asset to 3DStoryteller Studio")]
        private static void ExportPrefab(MenuCommand menuCommand)
        {
            if (Selection.objects == null || Selection.objects.Length == 0 || Selection.objects.Length > 1)
            {
                Debug.LogError("Please right-click on a prefab or scene to export the asset.");
                Debug.LogError("You must first set up the asset using the export tools.");
                ShowInstructionsLink();
                return;
            }

            UnityEngine.Object obj = Selection.objects[0];
            string assetPath = AssetDatabase.GetAssetPath(obj);
            string assetType = "";
            
            if (obj is SceneAsset)
            {
                assetType = "environment";
            }
            else if (obj is GameObject)
            {
                GameObject go = (GameObject)obj;
                if (go.GetComponent<Prop>())
                {
                    assetType = "prop";
                }
            }

            if (string.IsNullOrEmpty(assetType))
            {
                Debug.LogError("Unable to determine asset type for " + obj.name);
                ShowInstructionsLink();
                return;
            }
            
            string displayName = obj.name;
            string assetName = displayName.ToLower().Replace(" ", "_");
            AssetBundleBuilder.ExportAsset(assetName, assetPath);

            SessionState.SetBool("ThumbnailCaptureRequested", true);
            SessionState.SetString("displayName", displayName);
            SessionState.SetString("assetName", assetName);
            SessionState.SetString("assetType", assetType);
            
            if (assetType == "prop")
            {
                EditorSceneManager.OpenScene(propExportScenePath);
            }
            else
            {
                EditorSceneManager.OpenScene(assetPath);
                EditorSceneManager.OpenScene(envExportScenePath, OpenSceneMode.Additive);
                
            }
            
            //EditorApplication.EnterPlaymode();
            EditorApplication.isPlaying = true;
        }

        static void ShowInstructionsLink()
        {
            Debug.LogError("For instructions please see the documentation at https://www.3dstoryteller.studio/documentation/custom-assets.");
        }

        static void FinalizeAssetPackaging()
        {
            EditorApplication.isPlaying = false;
            
            AssetInfo info = new AssetInfo
            {
                dbname = SessionState.GetString("assetName", null),
                displayName = SessionState.GetString("displayName", null),
                type = SessionState.GetString("assetType", null)
            };

            string json = JsonUtility.ToJson(info);
            string dir = "Temp/" + info.dbname + "/" + info.dbname + "/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string jsonFile = dir + info.dbname + ".json";
            File.WriteAllText(jsonFile, json);

            string exportDir = "Assets/3DStoryteller/Export";
            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }
            string assetBundleFile = Path.Combine(exportDir, info.dbname + ".assetbundle");

            if (File.Exists(assetBundleFile))
                File.Delete(assetBundleFile);
            ZipFile.CreateFromDirectory(dir, assetBundleFile);
            
            Directory.Delete("Temp/" + info.dbname, true);
            AssetDatabase.Refresh();
        }
    }
}
