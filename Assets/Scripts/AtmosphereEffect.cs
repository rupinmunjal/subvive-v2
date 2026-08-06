using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AtmosphereEffect : MonoBehaviour
{
    public Shader atmosphereShader;

    public Color tintColor = new Color(0.5f, 0.8f, 0.9f, 1f);
    [Range(0f, 1f)] public float tintStrength = 0.15f;

    private Material material;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (atmosphereShader == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (material == null)
        {
            material = new Material(atmosphereShader);
            material.hideFlags = HideFlags.HideAndDontSave;
        }

        material.SetColor("_TintColor", tintColor);
        material.SetFloat("_TintStrength", tintStrength);

        Graphics.Blit(source, destination, material);
    }

    void OnDestroy()
    {
        if (material != null)
            DestroyImmediate(material);
    }
}
