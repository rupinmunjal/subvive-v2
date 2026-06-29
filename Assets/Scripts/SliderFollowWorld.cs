using UnityEngine;
using UnityEngine.UI;

public class SliderFollowWorld : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, -1f, 0);

    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = FindFirstObjectByType<Canvas>();
    }

    void Update()
    {
        if (target == null || canvas == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localPoint
        );
        rectTransform.localPosition = localPoint;
    }
}