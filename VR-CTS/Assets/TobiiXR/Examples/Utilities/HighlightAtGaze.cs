// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.G2OM;
using UnityEngine;

namespace Tobii.XR.Examples
{
    //Monobehaviour which implements the "IGazeFocusable" interface, meaning it will be called on when the object receives focus
    public class HighlightAtGaze : MonoBehaviour, IGazeFocusable
    {
        public Color HighlightColor = Color.red;
        public float AnimationTime = 0.1f;
        private float waitTime = 8.0f;
        private float timer = 0.0f;
        private float visualTime = 0.0f;

        private Renderer _renderer;
        private Color _originalColor;
        private Color _targetColor;


        //The method of the "IGazeFocusable" interface, which will be called when this object receives or loses focus
        public void GazeFocusChanged(bool hasFocus)
        {

            //If this object received focus, fade the object's color to highlight color
            if (hasFocus)
            {
                _targetColor = HighlightColor;
            }
            //If this object lost focus, fade the object's color to it's original color
            else
            {
                _targetColor = _originalColor;
            }
        }

        private void Start()
        {

            _renderer = GetComponent<Renderer>();
            _originalColor = _renderer.material.color;
            _targetColor = _originalColor;


        }

        private void Update()
        {
            // Check whether Tobii XR has any focused objects.
            if (TobiiXR.FocusedObjects.Count > 0)
            {
                // The object being focused by the user, determined by G2OM.
                var focusedObject = TobiiXR.FocusedObjects[0];
                Debug.Log("What am I looking at " + focusedObject.GameObject.name);
                Debug.Log("The object is at " + focusedObject.Direction);
            }
            // Get eye tracking data in world space
            var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

            // Check if gaze ray is valid
            if (eyeTrackingData.GazeRay.IsValid)
            {
                // The origin of the gaze ray is a 3D point
                var rayOrigin = eyeTrackingData.GazeRay.Origin;
                Debug.Log("Origin of the gaze ray" + rayOrigin.ToString("G4"));

                // The direction of the gaze ray is a normalized direction vector
                var rayDirection = eyeTrackingData.GazeRay.Direction;
                Debug.Log("Direction of the gaze ray" + rayOrigin.ToString("G4"));
            }
            //timer += Time.deltaTime;
            //if (timer > waitTime)
            //{
            //    visualTime = timer;
            //    timer = timer - waitTime;

            //    //This lerp will fade the color of the object
            //    if (_renderer.material.HasProperty(Shader.PropertyToID("_BaseColor"))) // new rendering pipeline (lightweight, hd, universal...)
            //    {
            //        _renderer.material.SetColor("_BaseColor", Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor, 2 * (1 / AnimationTime)));
            //    }
            //    else // old standard rendering pipline
            //    {
            //        _renderer.material.color = Color.Lerp(_renderer.material.color, _targetColor, 2 * (1 / AnimationTime));

            //    }

            //}
            //private void Update()
            //{
            //This lerp will fade the color of the object
            //if (_renderer.material.HasProperty(Shader.PropertyToID("_BaseColor"))) // new rendering pipeline (lightweight, hd, universal...)
            //{
            // _renderer.material.SetColor("_BaseColor", Color.Lerp(_renderer.material.GetColor("_BaseColor"), _targetColor, Time.deltaTime * (1 / AnimationTime)));
            // }
            // else // old standard rendering pipline
            // {
            //   _renderer.material.color = Color.Lerp(_renderer.material.color, _targetColor, Time.deltaTime * (1 / AnimationTime));
            //}
        }
    }
}
