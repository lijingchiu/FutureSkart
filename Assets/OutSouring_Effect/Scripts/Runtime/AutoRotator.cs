using UnityEditor;
using UnityEngine;

namespace OutSouring_Effect.Scripts.Runtime
{
    [ExecuteAlways] // Ensures execution in Edit Mode, Prefab Mode, and Play Mode
    public class AutoRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("Rotation speed in degrees per second for each axis (X, Y, Z).")]
        public Vector3 rotationSpeed = new Vector3(0f, 60f, 0f);

        [Tooltip("Coordinate system: Self uses local axes, World uses global axes.")]
        public Space rotationSpace = Space.Self;

        [Header("Editor Preview")]
        [Tooltip("Enable or disable rotation preview while in Edit Mode.")]
        public bool previewInEditor = true;

        void Update()
        {
            // 1. Play Mode: Standard rotation logic
            if (Application.isPlaying)
            {
                ApplyRotation();
            }
            // 2. Edit Mode: Rotate only if preview is enabled
            else if (previewInEditor)
            {
                ApplyRotation();
                
                // Key: Force Unity Editor to refresh every frame.
                // Without this, rotation in Edit Mode will stutter (only updating on mouse movement or scene changes).
#if UNITY_EDITOR
                EditorApplication.QueuePlayerLoopUpdate();
#endif
            }
        }

        private void ApplyRotation()
        {
            // Use Time.deltaTime to ensure frame-rate independent rotation speed
            transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpace);
        }
    }
}