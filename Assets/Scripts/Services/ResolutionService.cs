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
        GenerateResolutions();
        _hasInit = true;
    }

    private void GenerateResolutions()
    {
        _resolutions = new List<Resolution>();
        //var maxRes = Screen.resolutions.Last();
        var maxRes = Screen.currentResolution;
        int height = maxRes.height;
        _resolutions.Insert(0, new Resolution() { height = height, width = (int)(height * 0.5625f) });
        height -= height % 10;
        while (_resolutions.Count < 9)
        {
            var width = height * 0.5625f;
            var widthString = width.ToString("F3");
            var indexOfDot = widthString.IndexOf('.');
            if (widthString[indexOfDot + 1] == '0' && widthString[indexOfDot + 2] == '0' && width % 5 == 0)
                _resolutions.Insert(0, new Resolution() { height = height, width = (int)width });
            height -= 10;
        }
    }

    public List<Resolution> GetResolutions()
    {
        if (!_hasInit)
            Init();
        return _resolutions;
    }

    public void SetResolution(int resId)
    {
        if (!_hasInit)
            Init();
        Screen.SetResolution(_resolutions[resId].width, _resolutions[resId].height, PlayerPrefsHelper.GetFullscreen(), 60);
    }
}
