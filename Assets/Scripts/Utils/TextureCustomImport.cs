using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

namespace Utils
{
    public class TextureCustomImport : AssetPostprocessor
    {
        private class InnerSpriteMesh
        {
            public Dictionary<string, InnerMesh> Meshes;

            public void Apply(Sprite sprite)
            {
                if (Meshes.ContainsKey(sprite.name))
                {
                    Meshes[sprite.name].Apply(sprite);
                    Meshes[sprite.name].Dispose();
                    Meshes.Remove(sprite.name);
                }
            }
        }
        
        private class InnerMesh
        {
            private Vector2[] _mVertexes; 
            private ushort[] _mIndexes;
            
            public void InitVertexes(string line)
            {
                var lineParams = line.Split(';');
                var vertexCount = int.Parse(lineParams[0]);
                _mVertexes = new Vector2[vertexCount];

                for (var i = 0; i < _mVertexes.Length; ++i)
                {
                    var k = i * 2 + 1;
                    _mVertexes[i] = new Vector2(
                        float.Parse(lineParams[k]),
                        float.Parse(lineParams[k + 1])
                    );
                }
            }

            public void InitIndexes(string line)
            {
                var lineParams = line.Split(';');
                var indexCount = int.Parse(lineParams[0]);
                _mIndexes = new ushort[indexCount * 3];

                for (var i = 0; i < indexCount; ++i)
                {
                    var k = i * 3 + 1;

                    _mIndexes[i * 3] = ushort.Parse(lineParams[k]);
                    _mIndexes[i * 3 + 1] = ushort.Parse(lineParams[k + 1]);
                    _mIndexes[i * 3 + 2] = ushort.Parse(lineParams[k + 2]);
                }
            }

            public void Dispose()
            {
                _mIndexes = null;
                _mVertexes = null;
            }

            public void Apply(Sprite sprite)
            {
                sprite.OverrideGeometry(_mVertexes, _mIndexes);
            }
        }

        private static Dictionary<string, InnerSpriteMesh> _mTextures = new Dictionary<string, InnerSpriteMesh>();

        public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
        {
            if (!_mTextures.ContainsKey(assetPath)) return;
            
            var textureMeshes = _mTextures[assetPath];
            
            foreach (var sprite in sprites)
            {
                textureMeshes.Apply(sprite);
            }
            
            _mTextures.Remove(assetPath);
        }
        
        void OnPreprocessAsset()
        {
            if (Path.GetExtension(assetPath).Equals(".tpsheet"))
            {
                var texturePath = Path.ChangeExtension(assetPath, ".png");
                if (!File.Exists(texturePath)) return;
                
                if (!_mTextures.ContainsKey(texturePath))
                {
                    _mTextures.Add(texturePath, new InnerSpriteMesh
                    {
                        Meshes = new Dictionary<string, InnerMesh>()
                    });
                }
                var textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                if (textureImporter == null) return;
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;

                var textureMeshes = _mTextures[texturePath];
                var spritesheet = new List<SpriteMetaData>();
                var lines = File.ReadAllLines(assetPath);
                foreach (var line in lines)
                {
                    var lineChunks = line.Split(' ');
                    if (lineChunks.Length != 5) continue;
                        
                    var spriteMetaData = new SpriteMetaData { alignment = 9 };
                    {
                        var lineParams = lineChunks[0].Split(';');

                        spriteMetaData.name = lineParams[0];
                        spriteMetaData.rect = new Rect(
                            int.Parse(lineParams[1]),
                            int.Parse(lineParams[2]),
                            int.Parse(lineParams[3]),
                            int.Parse(lineParams[4])
                        );
                    }
                    {
                        var lineParams = lineChunks[1].Split(';');
                        spriteMetaData.pivot = new Vector2(float.Parse(lineParams[0], CultureInfo.InvariantCulture),
                            float.Parse(lineParams[1], CultureInfo.InvariantCulture));
                    }

                    var innerMesh = new InnerMesh();
                    innerMesh.InitVertexes(lineChunks[3]);
                    innerMesh.InitIndexes(lineChunks[4]);
                        
                    textureMeshes.Meshes.Add(spriteMetaData.name, innerMesh);
                        
                    spritesheet.Add(spriteMetaData);
                }

                textureImporter.spritesheet = spritesheet.ToArray();
                EditorUtility.SetDirty(textureImporter);
            
                textureImporter.SaveAndReimport();
            }
        }
    }
}