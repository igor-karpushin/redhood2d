
using System.Collections.Generic;
using System.Globalization;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomEditor(typeof(AtlasParser))]
    public class AtlasParserEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Convert"))
            {
                var targetClass = (AtlasParser) target;
                ParseAtlas(targetClass);
            }
        }

        private void ParseAtlas(AtlasParser parser)
        {
            var assetPath = AssetDatabase.GetAssetPath(parser.Texture);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);

        }
        
        
        
        
        
        
        
        
        
        
        
        
    }
}