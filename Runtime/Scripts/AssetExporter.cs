using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

namespace StorytellerStudio
{
    public class AssetExporter : MonoBehaviour
    {
        public ThumbnailGenerator thumbnailGenerator;

        private bool shouldCapture = false;
        
        private void Start()
        {
            
#if UNITY_EDITOR
            shouldCapture = SessionState.GetBool("ThumbnailCaptureRequested", false);
#endif
            if (shouldCapture)
            {
                StartCoroutine(Capture());
            }
        }

        IEnumerator Capture()
        {
            string assetName = "";
            string displayName = "";
            string assetType = "";

#if UNITY_EDITOR
            SessionState.SetBool("ThumbnailCaptureRequested", false);
#endif         
            
            //wait a few frames to make sure scene is playing
            yield return new WaitForEndOfFrame();
            yield return null;
            yield return null;
            
            View view = FindFirstObjectByType<View>();
            if (view != null)
            {
                displayName = SceneManager.GetActiveScene().name;
                assetName = displayName.ToLower().Replace(" ", "_");
                assetType = "environment";

                thumbnailGenerator.Generate(assetName, assetType);
            }
            else
            {
                Prop prop = FindFirstObjectByType<Prop>();

                if (prop == null)
                {
                    Debug.LogError("No asset found.");
                    Debug.LogError(
                        "For instructions please see the documentation at https://www.3dstoryteller.studio/documentation/custom-assets.");
                }
                else
                {
                    displayName = prop.gameObject.name;
                    assetName = displayName.ToLower().Replace(" ", "_");
                    assetType = "prop";

                    thumbnailGenerator.Generate(assetName, assetType);
                }
            }
            
#if UNITY_EDITOR
            SessionState.SetBool("ThumbnailCaptureCompleted", true);
            
            //wait an extra frame to ensure playback is initialized before stopping
            yield return null;
            
            EditorApplication.isPlaying = false;
#endif
        }
    }
}
