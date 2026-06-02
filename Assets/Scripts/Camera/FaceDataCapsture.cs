using UnityEngine;
using Mediapipe;
using Mediapipe.Unity.Sample.FaceLandmarkDetection;

/// <summary>
/// Captures head tilt (roll) and blink data from MediaPipe face landmarks.
/// Provides simple accessors and optional console logging.
/// </summary>
[RequireComponent(typeof(FaceLandmarkerRunner))]
public class FaceDataCapture : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Log head tilt and blink to console every N seconds (0 = disabled)")]
    public float logIntervalSeconds = 2f;

    [Tooltip("Eye Aspect Ratio below this is considered a blink")]
    public float blinkThreshold = 0.2f;

    // Runtime data
    private FaceLandmarkerRunner runner;
    private float lastLogTime;
    private float currentHeadTiltDegrees = 0f;
    private bool isBlinking = false;

    // === Public getters ===
    public float GetHeadTiltDegrees() => currentHeadTiltDegrees;
    public bool GetIsBlinking() => isBlinking;

    void Start()
    {
        runner = GetComponent<FaceLandmarkerRunner>();
        if (runner == null)
        {
            Debug.LogError("FaceDataCapture: No FaceLandmarkerRunner found on this GameObject.");
            enabled = false;
        }
    }

    void Update()
    {
        if (runner?.LatestResult?.faceLandmarks == null || runner.LatestResult.faceLandmarks.Count == 0)
            return;

        var landmarks = runner.LatestResult.faceLandmarks[0].Landmark;
        if (landmarks.Count < 468) return;

        // 1. Head tilt (roll) from eye corners (indices 33 = left, 263 = right)
        float dy = landmarks[263].Y - landmarks[33].Y;
        float dx = landmarks[263].X - landmarks[33].X;
        currentHeadTiltDegrees = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        // 2. Blink detection using Eye Aspect Ratio (EAR)
        float leftEAR = EyeAspectRatio(landmarks, 33, 159, 145, 133);
        float rightEAR = EyeAspectRatio(landmarks, 263, 386, 374, 362);
        float avgEAR = (leftEAR + rightEAR) * 0.5f;
        isBlinking = avgEAR < blinkThreshold;

        // Optional periodic logging
        if (logIntervalSeconds > 0 && Time.time - lastLogTime >= logIntervalSeconds)
        {
            lastLogTime = Time.time;
            Debug.Log($"[FaceData] Head Tilt: {currentHeadTiltDegrees:F1}° | Blink: {(isBlinking ? "YES" : "NO")} (EAR={avgEAR:F3})");
        }
    }

    // Calculate Eye Aspect Ratio using 4 points (simplified but effective)
    private float EyeAspectRatio(NormalizedLandmark[] lm, int outer, int upper, int lower, int inner)
    {
        Vector2 upperPoint = new Vector2(lm[upper].X, lm[upper].Y);
        Vector2 lowerPoint = new Vector2(lm[lower].X, lm[lower].Y);
        Vector2 outerPoint = new Vector2(lm[outer].X, lm[outer].Y);
        Vector2 innerPoint = new Vector2(lm[inner].X, lm[inner].Y);

        float vertical = Vector2.Distance(upperPoint, lowerPoint);
        float horizontal = Vector2.Distance(outerPoint, innerPoint);
        return horizontal > 0 ? vertical / horizontal : 0.5f;
    }
}