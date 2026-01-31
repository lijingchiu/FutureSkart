using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UberParticleShader.Runtime
{
    public class ShowCaseController : MonoBehaviour
    {
        //* Main Data
        public List<GameObject> Particles;
        [SerializeField] private List<PlayableDirector> _playableDirectors;

        //* Performance
        private float _deltaTime;
        public bool _showFPSInfo;

        //* GUI Data
        private Texture _nextIcon;
        private Texture _previousIcon;
        private Texture _rePlayIcon;
        private Texture _gradientIcon;
        private Texture _30Icon;
        private Texture _60Icon;
        private Texture _120Icon;
        private int _currentParticleIndex;
        private Vector2 _screenParams;
        private GUIStyle _boldTitle_MiddleCenter_GUIStyle;
        private GUIStyle _boldTitle_LeftUpper_GUIStyle;
        private readonly Color _defaultTextColor = new Color(0.7f, 0.7f, 0.7f, 1);

        private void Start()
        {
            foreach (var particle in Particles)
                if (particle.TryGetComponent(out PlayableDirector playableDirector))
                    _playableDirectors.Add(playableDirector);

            _nextIcon = Resources.Load<Texture>("Next");
            _previousIcon = Resources.Load<Texture>("Previous");
            _rePlayIcon = Resources.Load<Texture>("Refresh");
            _gradientIcon = Resources.Load<Texture>("Gradient");
            _30Icon = Resources.Load<Texture>("FPS_30");
            _60Icon = Resources.Load<Texture>("FPS_60");
            _120Icon = Resources.Load<Texture>("FPS_120");

            _boldTitle_MiddleCenter_GUIStyle = new GUIStyle { alignment = TextAnchor.MiddleCenter, normal = { textColor = _defaultTextColor }, fontSize = 48 };
            _boldTitle_LeftUpper_GUIStyle = new GUIStyle { alignment = TextAnchor.UpperLeft, normal = { textColor = _defaultTextColor }, fontSize = 48 };
        }

        private void Update()
        {
            _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            _screenParams = new Vector2(Screen.width, Screen.height);
            ShowOperation();
            DisplayCurrentParticleName();
            DrawSwitchFrameRate();
            ShowPerformance();
        }

        private void ShowOperation()
        {
            // Set Particles
            GUI.DrawTexture(new Rect(0, _screenParams.y - 100, _screenParams.x, 100), _gradientIcon);

            if (GUI.Button(new Rect(_screenParams.x * 0.25f, _screenParams.y - 100, 100, 100), _previousIcon))
                SwitchToPreviousParticle();

            if (GUI.Button(new Rect(_screenParams.x * 0.75f, _screenParams.y - 100, 100, 100), _nextIcon))
                SwitchToNextParticle();

            if (GUI.Button(new Rect(_screenParams.x - 100, _screenParams.y - 100, 100, 100), _rePlayIcon))
                EnableAndPlayParticle(_currentParticleIndex);
        }

        private void DrawSwitchFrameRate()
        {
            if (_showFPSInfo)
            {
                if (GUI.Button(new Rect(_screenParams.x - 100, 0, 100, 100), _30Icon))
                    SwitchFrameRate(30);

                if (GUI.Button(new Rect(_screenParams.x - 100, 100, 100, 100), _60Icon))
                    SwitchFrameRate(60);

                if (GUI.Button(new Rect(_screenParams.x - 100, 200, 100, 100), _120Icon))
                    SwitchFrameRate(120);
            }
        }

        private void SwitchFrameRate(int frameRate)
        {
            Application.targetFrameRate = frameRate;
        }

        private void DisplayCurrentParticleName()
        {
            GUI.Label(new Rect(_screenParams.x * 0.5f, _screenParams.y - 100, 100, 100), Particles[_currentParticleIndex].name, _boldTitle_MiddleCenter_GUIStyle);
        }

        private void SwitchToNextParticle()
        {
            StopPlayParticle(_currentParticleIndex);
            _currentParticleIndex = (_currentParticleIndex + 1) % Particles.Count;
            EnableAndPlayParticle(_currentParticleIndex);
        }

        private void SwitchToPreviousParticle()
        {
            StopPlayParticle(_currentParticleIndex);
            _currentParticleIndex--;
            if (_currentParticleIndex < 0)
                _currentParticleIndex = Particles.Count - 1;
            EnableAndPlayParticle(_currentParticleIndex);
        }

        private void EnableAndPlayParticle(int currentParticleIndex)
        {
            Particles[currentParticleIndex].SetActive(true);
            var clip = _playableDirectors[currentParticleIndex].playableAsset;
            _playableDirectors[currentParticleIndex].Play(clip, DirectorWrapMode.Hold);
        }

        private void StopPlayParticle(int currentParticleIndex)
        {
            Particles[currentParticleIndex].SetActive(false);
            _playableDirectors[currentParticleIndex].Stop();
        }

        private void ShowPerformance()
        {
            if (_showFPSInfo)
            {
                // FPS
                var msec = _deltaTime * 1000.0f;
                var fps = 1.0f / _deltaTime;
                var FPSInfo = $"Performance: {msec:0.0} ms ({fps:0.} fps)";
                GUI.Label(new Rect(0, 0, 100, 100), FPSInfo, _boldTitle_LeftUpper_GUIStyle);

                // User Graphic Card
                GUI.Label(new Rect(0, 50, 100, 100), $"Graphic Card: {SystemInfo.graphicsDeviceName}", _boldTitle_LeftUpper_GUIStyle);
                GUI.Label(new Rect(0, 100, 100, 100), $"Graphic API: {SystemInfo.graphicsDeviceType}", _boldTitle_LeftUpper_GUIStyle);
            }
        }
    }
}