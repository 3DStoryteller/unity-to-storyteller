using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StorytellerStudio
{
    public class View : MonoBehaviour
    {
        [Header("Legacy - Do Not Use")]
        [SerializeField] private Transform avatarTransform;
        [SerializeField] private Transform propTransform;
        [SerializeField] private Transform lightsTransform;

        public Vector3 pos { get { return transform.localPosition; } }
        public Vector3 rot { get { return transform.localEulerAngles; } }

        //todo - legacy
        public bool isLegacy { get { return avatarTransform != null; } }
        public Vector3 avatarPos { get { return avatarTransform.localPosition; } }
        public Vector3 avatarRot { get { return avatarTransform.localEulerAngles; } }
        public Vector3 propPos { get { return propTransform.localPosition; } }
        public Vector3 propRot { get { return propTransform.localEulerAngles; } }
        public Vector3 lightsPos { get { return lightsTransform.localPosition; } }
        public Vector3 lightsRot { get { return lightsTransform.localEulerAngles; } }
    }
}