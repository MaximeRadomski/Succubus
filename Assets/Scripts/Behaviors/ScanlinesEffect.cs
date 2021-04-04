using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Camera/Scanlines Effect")]
public class ScanlinesEffect : MonoBehaviour
{
    public Shader shader;
    private Material _material;

    [Range(0, 10)]
    private float lineWidth = 0.4f;

    [Range(0, 10)]
    private float columnWidth = 0.4f;

    [Range(0, 1)]
    private float hardness;

    [Range(0, 1)]
    private float displacementSpeed = 0.0f;

    private void Start()
    {
        UpdateHardness();
    }

    public void UpdateHardness()
    {
        var tmpHardness = PlayerPrefsHelper.GetScanlinesHardness() / 7.0f;
        hardness = tmpHardness;
    }

    protected Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null)
            return;
        material.SetFloat("_LineWidth", lineWidth);
        material.SetFloat("_ColumnWidth", columnWidth);
        material.SetFloat("_Hardness", hardness);
        material.SetFloat("_Speed", displacementSpeed);
        Graphics.Blit(source, destination, material, 0);
    }

    void OnDisable()
    {
        if (_material)
        {
            DestroyImmediate(_material);
        }
    }
}
