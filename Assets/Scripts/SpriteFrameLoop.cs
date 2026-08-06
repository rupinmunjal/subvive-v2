using UnityEngine;
using UnityEngine.UI;

public class SpriteFrameLoop : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 8f;

    private SpriteRenderer spriteRenderer;
    private Image image;
    private int frameIndex;
    private float timer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
    }

    void OnEnable()
    {
        frameIndex = 0;
        timer = 0f;
        if (frames != null && frames.Length > 0)
            SetFrame(frames[0]);
    }

    void Update()
    {
        if (frames == null || frames.Length < 2) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            frameIndex = (frameIndex + 1) % frames.Length;
            SetFrame(frames[frameIndex]);
        }
    }

    private void SetFrame(Sprite sprite)
    {
        if (spriteRenderer != null) spriteRenderer.sprite = sprite;
        if (image != null) image.sprite = sprite;
    }
}
