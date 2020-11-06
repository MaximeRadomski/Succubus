using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResolutionService : MonoBehaviour
{
    private bool _hasInit;
    private List<Resolution> _resolutions;

    private void Init()
    {
        GenerateResolutions(10, 10);
        if (_resolutions.Count < 9)
            GenerateResolutions(10, 5);
        if (_resolutions.Count < 9)
            GenerateResolutions(5, 5);
        if (_resolutions.Count < 9)
            GenerateResolutions(5, 2);
        if (_resolutions.Count < 9)
            GenerateResolutions(2, 2);
        _hasInit = true;
    }

    private void GenerateResolutions(int baseUnitHeight, int baseUnitWidth)
    {
        if (_resolutions != null)
            _resolutions.Clear();
        else
            _resolutions = new List<Resolution>();
        //var maxRes = Screen.resolutions.Last();
        var maxRes = Screen.currentResolution;
        int height = maxRes.height;
        _resolutions.Insert(0, new Resolution() { height = height, width = (int)(height * 0.5625f) });
        height = (height % baseUnitHeight) != height ? height - (height % baseUnitHeight) : height - baseUnitHeight;
        while (_resolutions.Count < 9 && height > 0)
        {
            if (height == 800)
                height = 800;
            var width = height * 0.5625f;
            var widthString = width.ToString("F3");
            widthString = widthString.Replace(',', '.');
            var indexOfDot = widthString.IndexOf('.');
            if (widthString[indexOfDot + 1] == '0' && widthString[indexOfDot + 2] == '0' && width % baseUnitWidth == 0)
                _resolutions.Insert(0, new Resolution() { height = height, width = (int)width });
            height -= baseUnitHeight;
        }
    }

    public List<Resolution> GetResolutions()
    {
        if (!_hasInit)
            Init();
        return _resolutions;
    }

    public int SetResolution(int resId)
    {
        if (!_hasInit)
            Init();
        if (resId >= _resolutions.Count)
            resId = _resolutions.Count - 1;
        Screen.SetResolution(_resolutions[resId].width, _resolutions[resId].height, PlayerPrefsHelper.GetFullscreen(), 60);
        return resId;
    }
}
