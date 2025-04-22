using System.Collections;
using System.IO;
using UnityEngine;

namespace StorytellerStudio
{
    public class ThumbnailGenerator : MonoBehaviour
    {
        public Camera mainCam;

        GameObject theObj;
        Transform center;
        BoxCollider genCollider;
        Bounds combinedBounds;
        Camera renderCam;
        string curScenePath;
        
        public void Generate(string assetName, string assetType)
        {
            if (assetType == "prop")
            {
                theObj = FindFirstObjectByType<Prop>().gameObject;

                //make sure only the default state is showing
                PropStateContainer psc = theObj.GetComponentInChildren<PropStateContainer>();
                if (psc)
                {
                    foreach (PropState propState in theObj.GetComponentsInChildren<PropState>())
                    {
                        propState.gameObject.SetActive(propState == psc.defaultState);
                    }
                }

                //disable any animations
                Animator anim = theObj.GetComponentInChildren<Animator>();
                if (anim)
                {
                    StartCoroutine(StopAnimator(anim));
                }

                //get bounds
                CreateCollider(theObj);

                //focus the camera on the center by moving the rig there 
                mainCam.transform.parent.position = center.position;
                
                while (!InView())
                {
                    Vector3 pos = mainCam.transform.localPosition;
                    pos.z -= 0.01f;
                    mainCam.transform.localPosition = pos;
                }
            }

            CamCapture(assetName);
        }

        IEnumerator StopAnimator(Animator anim)
        {
            yield return new WaitForEndOfFrame();
            anim.enabled = false;
        }

        bool InView()
        {
            Vector3 left = combinedBounds.max;
            Debug.DrawLine(center.position, left, Color.red);
            Vector3 line = left - center.position;
            line.z = -line.z;
            Vector3 top = center.position + line;
            Debug.DrawLine(center.position, top, Color.cyan);
            line.x = -line.x;
            Vector3 right = center.position + line;
            Debug.DrawLine(center.position, right, Color.black);
            line.y = -line.y;
            line.z = -line.z;
            Vector3 bottom = center.position + line;
            Debug.DrawLine(center.position, bottom, Color.yellow);

            Vector3 viewLeft = mainCam.WorldToViewportPoint(left);
            Vector3 viewRight = mainCam.WorldToViewportPoint(right);
            Vector3 viewTop = mainCam.WorldToViewportPoint(top);
            Vector3 viewBottom = mainCam.WorldToViewportPoint(bottom);

            return
                viewLeft.x > 0 && viewLeft.x < 1 && viewLeft.y > 0 && viewLeft.y < 1 &&
                viewRight.x > 0 && viewRight.x < 1 && viewRight.y > 0 && viewRight.y < 1 &&
                viewTop.x > 0 && viewTop.x < 1 && viewTop.y > 0 && viewTop.y < 1 &&
                viewBottom.x > 0 && viewBottom.x < 1 && viewBottom.y > 0 && viewBottom.y < 1;
        }

        public void CreateCollider(GameObject go)
        {
            // Initialize an empty bounds structure
            combinedBounds = new Bounds(Vector3.zero, Vector3.zero);
            bool hasBounds = false;

            // Iterate through all child MeshFilters
            foreach (MeshFilter meshFilter in go.GetComponentsInChildren<MeshFilter>())
            {
                Bounds worldBounds = new Bounds();

                if (meshFilter.sharedMesh != null)
                {
                    // Get the mesh bounds in local space
                    Mesh mesh = meshFilter.sharedMesh;
                    Bounds meshBounds = mesh.bounds;

                    // Calculate the bounds in world space
                    Vector3 worldMin = meshFilter.transform.TransformPoint(meshBounds.min);
                    Vector3 worldMax = meshFilter.transform.TransformPoint(meshBounds.max);

                    // Create new bounds in world space
                    worldBounds.SetMinMax(worldMin, worldMax);
                }
                else
                {
                    worldBounds = meshFilter.gameObject.AddComponent<BoxCollider>().bounds;
                }

                // Encapsulate the current mesh bounds into the combined bounds
                if (hasBounds)
                {
                    combinedBounds.Encapsulate(worldBounds.min);
                    combinedBounds.Encapsulate(worldBounds.max);
                }
                else
                {
                    combinedBounds = worldBounds;
                    hasBounds = true;
                }
            }

            foreach (SkinnedMeshRenderer smr in go.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (hasBounds)
                {
                    combinedBounds.Encapsulate(smr.bounds);
                }
                else
                {
                    combinedBounds = smr.bounds;
                    hasBounds = true;
                }
            }

            //add some padding
            combinedBounds.size *= 1.1f;

            // Add a BoxCollider and set its center and size
            genCollider = go.AddComponent<BoxCollider>();

            GameObject centerGo = new GameObject("Center");
            center = centerGo.transform;

            //if we found some meshes, create the collider
            if (hasBounds)
            {
                // Calculate the center and size in local space
                Vector3 localCenter = go.transform.InverseTransformPoint(combinedBounds.center);
                Vector3 localSize = go.transform.InverseTransformVector(combinedBounds.size);

                genCollider.center = localCenter;
                genCollider.size = localSize;

                centerGo.transform.position = combinedBounds.center;
            }
            else
            {
                Debug.LogError("Bounds not found");
            }
        }

        void CamCapture(string assetName)
        {
            foreach (Camera c in mainCam.GetComponentsInChildren<Camera>())
            {
                if (c.targetTexture != null)
                {
                    renderCam = c;
                    break;
                }
            }
            
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderCam.targetTexture;

            renderCam.Render();

            Texture2D image = new Texture2D(renderCam.targetTexture.width, renderCam.targetTexture.height);
            image.ReadPixels(new Rect(0, 0, renderCam.targetTexture.width, renderCam.targetTexture.height), 0, 0);
            image.Apply();
            RenderTexture.active = currentRT;

            var bytes = image.EncodeToPNG();
            Destroy(image);

            string dir = Application.dataPath + "/../Temp/" + assetName + "/" + assetName + "/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path = dir + assetName + ".png";
            File.WriteAllBytes(path, bytes);
        }
    }
}