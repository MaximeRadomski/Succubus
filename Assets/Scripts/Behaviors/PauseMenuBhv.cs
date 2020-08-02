using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuBhv : MonoBehaviour
{
    public List<ButtonBhv> Buttons;
    public List<TMPro.TextMeshPro> TextMeshes;

    private List<PositionBhv> _borders;

    private Vector3 _cameraInitialPosition;

    public void SetPrivates()
    {
        Buttons = new List<ButtonBhv>();
        TextMeshes = new List<TMPro.TextMeshPro>();
        _borders = new List<PositionBhv>();
        _cameraInitialPosition = Camera.main.transform.position;
        var canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; ++i)
        {
            Buttons.Add(canvas.transform.GetChild(i).GetComponent<ButtonBhv>());
        }
        foreach (var button in Buttons)
        {
            TextMeshes.Add(button.gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        }
        var borders = transform.Find("Borders");
        for (int i = 0; i < borders.childCount; ++i)
        {
            _borders.Add(borders.transform.GetChild(i).GetComponent<PositionBhv>());
        }
        foreach (var border in _borders)
        {
            border.gameObject.SetActive(false);
        }
    }

    public void Pause()
    {
        Camera.main.transform.position = transform.position;
        foreach (var border in _borders)
        {
            border.gameObject.SetActive(true);
            border.UpdatePositions();
        }
    }

    public void UnPause()
    {
        foreach (var border in _borders)
        {
            border.gameObject.SetActive(false);
        }
        Camera.main.transform.position = _cameraInitialPosition;
    }
}
