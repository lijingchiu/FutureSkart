using UnityEngine;

namespace UberParticleShader.Editor
{
    [CreateAssetMenu(fileName = "UberParticleShaderEditorSetting", menuName = "UberParticle/UserPrefs")]
    internal class UberParticleEditorSetting : ScriptableObject
    {
        private static UberParticleEditorSetting _instance;

        public static UberParticleEditorSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<UberParticleEditorSetting>("UberParticle/UberParticleShaderEditorSetting");
                    if (_instance == null)
                    {
                        Debug.Log("UberParticle EditorSetting instance is not found in Resources folder. " +
                                  "Please create one at <color=#FF7D00>UberParticleShader/Editor/Resources/UberParticle</color>.");
                    }
                }

                return _instance;
            }
        }

        public Shader UberParticleShader;
        public bool AutoFixVertexStreams = true;
        public bool CollapseFeatures = true;
        public bool FinalOutput = true;
        public bool BlendSetting = true;
        public bool StencilSetting = true;
        public bool RenderQueueSetting = true;
        public bool GPUInstancingSetting = true;

        internal void OnEnable()
        {
            if (UberParticleShader == null)
            {
                UberParticleShader = Shader.Find("UberParticleShader");
            }
        }
    }
}