﻿using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace EDUCATION.FEELPHYSICS.MY_HOLOGRAPHIC_ACADEMY
{
    public class ColorChanger : MonoBehaviour, IInputClickHandler
    {
        #region Private Valuables

        private Material material;

        private bool isBlue;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            material = this.gameObject.GetComponent<Renderer>().material;
            material.SetColor("_Color", Color.blue);
            this.isBlue = true;
        }

        #endregion

        #region Public Methods

        public void OnInputClicked(InputClickedEventData eventData)
        {
            DebugLog.Instance.Log += "OnInputClicked\n";
            if (this.isBlue)
            {
                material.SetColor("_Color", Color.red);
            }
            else
            {
                material.SetColor("_Color", Color.blue);
            }

            this.isBlue = !this.isBlue;
        }

        #endregion
    }
}