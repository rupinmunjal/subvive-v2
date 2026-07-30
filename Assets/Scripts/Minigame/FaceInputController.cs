using UnityEngine;
using System;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.FaceLandmarker;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Unity.Sample.FaceLandmarkDetection;

namespace SubVive.Minigame
{
    public class FaceInputController : MonoBehaviour
    {
        [SerializeField] private FaceLandmarkerRunner runner; // drag the runner GameObject here in Inspector

        [Header("Tuning")]
        [SerializeField] private float tiltDeadzone = 0.05f;
        [SerializeField] private float tiltSensitivity = 4f;
        [SerializeField] private float blinkThreshold = 0.22f;
        [SerializeField] private float blinkCooldown = 0.4f;

        public float SteerValue { get; private set; }
        public event Action OnBlinkShoot;

        private float _lastBlinkTime = -999f;
        private bool _eyeWasClosed;
        private float _targetSteer;

        private float _lostTrackingTime = -1f;
        private const float TrackingGraceDuration = 0.45f;

        // Thread safety structures
        private struct FaceData
        {
            public Vector2 leftEyeOuter;
            public Vector2 rightEyeOuter;
            public Vector2 rightEyeTop1;
            public Vector2 rightEyeBottom1;
            public Vector2 rightEyeTop2;
            public Vector2 rightEyeBottom2;
            public Vector2 rightEyeInner;
            public bool isValid;
        }

        private readonly object _lock = new object();
        private FaceData _pendingFaceData;
        private bool _hasNewData;

        void OnEnable()
        {
            if (runner != null) runner.OnResultUpdated += HandleResult;
        }

        void OnDisable()
        {
            if (runner != null) runner.OnResultUpdated -= HandleResult;
        }

        void Update()
        {
            FaceData localData;
            bool gotNewData = false;

            lock (_lock)
            {
                if (_hasNewData)
                {
                    localData = _pendingFaceData;
                    _hasNewData = false;
                    gotNewData = true;
                }
                else
                {
                    localData = default;
                }
            }

            if (gotNewData)
            {
                if (localData.isValid)
                {
                    _lostTrackingTime = -1f;
                    UpdateHeadTilt(localData);
                    UpdateBlink(localData);
                }
                else
                {
                    if (_lostTrackingTime < 0f)
                    {
                        _lostTrackingTime = Time.time;
                    }
                }
            }

            // If we are currently in tracking loss
            if (_lostTrackingTime > 0f)
            {
                float timeLost = Time.time - _lostTrackingTime;
                if (timeLost >= TrackingGraceDuration)
                {
                    _targetSteer = Mathf.MoveTowards(_targetSteer, 0f, Time.deltaTime * 3f);
                }
            }

            SteerValue = Mathf.Lerp(SteerValue, _targetSteer, Time.deltaTime * tiltSensitivity);
        }

        private void HandleResult(FaceLandmarkerResult result)
        {
            if (result.faceLandmarks == null || result.faceLandmarks.Count == 0)
            {
                lock (_lock)
                {
                    _pendingFaceData = new FaceData { isValid = false };
                    _hasNewData = true;
                }
                return;
            }

            var landmarks = result.faceLandmarks[0].landmarks;
            if (landmarks == null || landmarks.Count < 264) return;

            FaceData data = new FaceData
            {
                leftEyeOuter = new Vector2(landmarks[33].x, landmarks[33].y),
                rightEyeOuter = new Vector2(landmarks[263].x, landmarks[263].y),
                rightEyeTop1 = new Vector2(landmarks[159].x, landmarks[159].y),
                rightEyeBottom1 = new Vector2(landmarks[145].x, landmarks[145].y),
                rightEyeTop2 = new Vector2(landmarks[158].x, landmarks[158].y),
                rightEyeBottom2 = new Vector2(landmarks[153].x, landmarks[153].y),
                rightEyeInner = new Vector2(landmarks[133].x, landmarks[133].y),
                isValid = true
            };

            lock (_lock)
            {
                _pendingFaceData = data;
                _hasNewData = true;
            }
        }

        private void UpdateHeadTilt(FaceData fd)
        {
            Vector2 left = fd.leftEyeOuter;
            Vector2 right = fd.rightEyeOuter;

            float dx = right.x - left.x;
            float dy = right.y - left.y;
            float rollAngle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

            float normalized = Mathf.Clamp(rollAngle / 25f, -1f, 1f);
            if (Mathf.Abs(normalized) < tiltDeadzone) normalized = 0f;

            _targetSteer = normalized;
        }

        private void UpdateBlink(FaceData fd)
        {
            float ear = ComputeEyeAspectRatio(fd);
            bool eyeClosed = ear < blinkThreshold;

            if (eyeClosed && !_eyeWasClosed && Time.time - _lastBlinkTime > blinkCooldown)
            {
                _lastBlinkTime = Time.time;
                OnBlinkShoot?.Invoke();
            }

            _eyeWasClosed = eyeClosed;
        }

        private float ComputeEyeAspectRatio(FaceData fd)
        {
            float vertical1 = Vector2.Distance(fd.rightEyeTop1, fd.rightEyeBottom1);
            float vertical2 = Vector2.Distance(fd.rightEyeTop2, fd.rightEyeBottom2);
            float horizontal = Vector2.Distance(fd.rightEyeOuter, fd.rightEyeInner);

            return (vertical1 + vertical2) / (2f * horizontal);
        }
    }
}