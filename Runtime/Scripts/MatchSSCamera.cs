using UnityEngine;

namespace StorytellerStudio
{
    public class MatchSSCamera : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            var view = FindFirstObjectByType<View>(FindObjectsInactive.Include);
            transform.position = view.transform.position;
            transform.rotation = view.transform.rotation;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}