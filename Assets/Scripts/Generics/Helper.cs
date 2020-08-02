﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class Helper
{
    public static float GetAngleFromTwoPositions(Vector3 start, Vector3 end)
    {
        float x = Mathf.Abs(start.x - end.x);
        float y = Mathf.Abs(start.y - end.y);
        float angle = 0.0f;
        if (FloatEqualsPrecision(end.x, start.x, 0.01f) && end.y > start.y) //UP
            angle = 0.0f;
        else if (FloatEqualsPrecision(end.y, start.y, 0.01f) && end.x > start.x) //RIGHT
            angle = 90.0f;
        else if (FloatEqualsPrecision(end.x, start.x, 0.01f) && end.y < start.y) //DOWN
            angle = 180.0f;
        else if (FloatEqualsPrecision(end.y, start.y, 0.01f) && end.x < start.x) //LEFT
            angle = 270.0f;
        else //DIAGONALS
        {
            
            if (end.y > start.y && end.x > start.x) //UP-RIGHT
            {
                angle = Mathf.Atan(x / y) * Mathf.Rad2Deg;
                angle += 0.0f;
            }
            else if (end.y < start.y && end.x > start.x) //DOWN-RIGHT
            {
                angle = Mathf.Atan(y / x) * Mathf.Rad2Deg;
                angle += 90.0f;
            }
            else if (end.y < start.y && end.x < start.x) //DOWN-LEFT
            {
                angle = Mathf.Atan(x / y) * Mathf.Rad2Deg;
                angle += 180.0f;
            }
            else if (end.y > start.y && end.x < start.x) //UP-LEFT
            {
                angle = Mathf.Atan(y / x) * Mathf.Rad2Deg;
                angle += 270.0f;
            }
        }
        return -angle;
    }

    public static string GetOrdinal(int number)
    {
        string str = number.ToString();
        var suffix = "th";
        if (str.Length == 1)
        {
            if (number == 1)
                suffix = "st";
            else if (number == 2)
                suffix = "nd";
            if (number == 3)
                suffix = "rd";
        }
        else
        {
            var last = str.Substring(str.Length - 1);
            var secondLast = str.Substring(str.Length - 2, 1);
            if (secondLast != "1")
            {
                if (last == "1")
                    suffix = "st";
                else if (last == "2")
                    suffix = "nd";
                if (last == "3")
                    suffix = "rd";
            }
        }
        return number + suffix;
    }

    public static IEnumerator ExecuteAfterDelay(float delay, Func<object> func, bool lockInputWhile = true)
    {
        if (lockInputWhile)
            Constants.InputLocked = true;
        yield return new WaitForSeconds(delay);
        func.Invoke();
        if (lockInputWhile)
            Constants.InputLocked = false;
    }

    public static int RandomIntMultipleOf(int min, int max, int multiple)
    {
        var unRoundedMinutes = UnityEngine.Random.Range(min, max + multiple);
        return unRoundedMinutes - unRoundedMinutes % multiple;
    }

    public static int CharacterAfterString(string str, string subStr)
    {
        if (!str.Contains(subStr))
            return -1;
        return str.IndexOf(subStr) + subStr.Length;
    }

    public static Sprite GetSpriteFromSpriteSheet(string path)
    {
        var separatorId = path.IndexOf('_');
        var spriteSheetPath = path.Substring(0, separatorId);
        var spriteSheet = Resources.LoadAll<Sprite>(spriteSheetPath);
        var spriteId = int.Parse(path.Substring(separatorId + 1));
        if (spriteId >= spriteSheet.Length)
            return null;
        else
            return spriteSheet[spriteId];
    }
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static int EnumCount<EnumType>()
    {
        return System.Enum.GetNames(typeof(EnumType)).Length;
    }

    public static string GetDescription(this Enum value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var description = value.ToString();
        var fieldInfo = value.GetType().GetRuntimeField(description);

        if (fieldInfo == null)
            return string.Empty;
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes.Length > 0)
        {
            description = attributes[0].Description;
        }

        return description;
    }

    public static float MultiplierFromPercent(float root, int percent)
    {
        return root + ((float)percent / 100.0f);
    }

    public static int RoundToNextDecade(int value)
    {
        return value + (value % 10 == 0 ? 0 : 10 - value % 10);
    }

    public static bool FloatEqualsPrecision(float float1, float float2, float precision)
    {
        return float1 >= float2 - precision && float1 <= float2 + precision;
    }

    public static bool VectorEqualsPrecision(Vector3 vector1, Vector3 vector2, float precision)
    {
        return (vector1.x >= vector2.x - precision && vector1.x <= vector2.x + precision)
            && (vector1.y >= vector2.y - precision && vector1.y <= vector2.y + precision)
            && (vector1.z >= vector2.z - precision && vector1.z <= vector2.z + precision);
    }

    public static bool ColorEqualsPrecision(Color color1, Color color2, float precision)
    {
        return (color1.r >= color2.r - precision && color1.r <= color2.r + precision)
            && (color1.g >= color2.g - precision && color1.g <= color2.g + precision)
            && (color1.b >= color2.b - precision && color1.b <= color2.b + precision)
            && (color1.a >= color2.a - precision && color1.a <= color2.a + precision);
    }
    public static string MaterialFromTextType(int id, TextThickness thickness)
    {
        TextType tmpType = (TextType)id;
        switch (tmpType)
        {
            case TextType.Normal:
                return thickness + "White";
            case TextType.Magical:
                return thickness + "Blue";
            case TextType.Rare:
                return thickness + "Yellow";
            case TextType.Legendary:
                return thickness + "Orange";
            case TextType.Hp:
                return thickness + "Red";
            case TextType.Pa:
                return thickness + "Blue";
            case TextType.Pm:
                return thickness + "Green";
            case TextType.Gold:
                return thickness + "Gold";
            case TextType.Xp:
                return thickness + "Orange";
            case TextType.HpCritical:
                return thickness + "RedCritical";
        }
        return thickness + "White";
    }

    public static void ReloadScene()
    {
        NavigationService.ReloadScene();
    }
}
