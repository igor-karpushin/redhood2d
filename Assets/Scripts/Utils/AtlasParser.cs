using System;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public class AtlasParser : MonoBehaviour
    {
        public Texture2D Texture;

        private void OnDrawGizmos()
        {
            var colors = new Color[9]
            {
                Color.blue,
                Color.green,
                Color.magenta,
                Color.red,
                Color.yellow,
                Color.cyan,
                Color.gray,
                Color.white,
                Color.black
            };

            
        }
    }
}