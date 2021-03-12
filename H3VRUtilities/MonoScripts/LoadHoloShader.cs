using System;
using UnityEngine;


namespace H3VRUtils.Shaders
{
    public class LoadHoloShader : MonoBehaviour
    {
        [Header("Texture Settings")]
        public Texture2D sightTexture;

        public Vector2 sightOffset;

        public Vector2 sightScale;

        public Color sightColour;
        
        private void Awake()
        {
            GetComponent<Renderer>().material = new Material(Shader.Find("H3VR/HolographicSight"))
            {
                mainTexture = sightTexture,
                renderQueue = 3001,
                mainTextureScale = sightScale,
                color = sightColour,
                mainTextureOffset = sightOffset
            };
        }
    }
}