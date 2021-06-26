using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class KeyCodeService
{
    public static List<KeyCode> ForbiddenTextKeys = new List<KeyCode>()
    {
        KeyCode.AltGr,
        KeyCode.LeftControl,
        KeyCode.RightControl,
        KeyCode.LeftAlt,
        KeyCode.RightAlt,
        KeyCode.Escape,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.UpArrow,
        KeyCode.Insert,
        KeyCode.Home,
        KeyCode.End,
        KeyCode.PageUp,
        KeyCode.PageDown,
        KeyCode.F1,
        KeyCode.F2,
        KeyCode.F3,
        KeyCode.F4,
        KeyCode.F5,
        KeyCode.F6,
        KeyCode.F7,
        KeyCode.F8,
        KeyCode.F9,
        KeyCode.F10,
        KeyCode.F11,
        KeyCode.F12,
        KeyCode.F13,
        KeyCode.F14,
        KeyCode.F15,
        KeyCode.Plus,
        KeyCode.KeypadPlus,
        KeyCode.Equals,
        KeyCode.KeypadEquals,
        KeyCode.Hash,
        KeyCode.Dollar,
        KeyCode.Percent,
        KeyCode.Ampersand,
        KeyCode.Less,
        KeyCode.Greater,
        KeyCode.Backslash,
        KeyCode.Caret,
        KeyCode.BackQuote,
        KeyCode.Pipe,
        KeyCode.Tilde,
        KeyCode.Numlock,
        KeyCode.Delete,
        KeyCode.Backspace,
        KeyCode.Clear,
        KeyCode.CapsLock,
        KeyCode.LeftShift,
        KeyCode.RightShift,
        KeyCode.Return,
        KeyCode.KeypadEnter,
        KeyCode.Tab,
        KeyCode.ScrollLock,
        KeyCode.LeftCommand,
        KeyCode.LeftApple,
        KeyCode.LeftWindows,
        KeyCode.RightCommand,
        KeyCode.RightApple,
        KeyCode.RightWindows,
        KeyCode.Help,
        KeyCode.Print,
        KeyCode.SysReq,
        KeyCode.Menu,
        KeyCode.Break
    };

    public static string KeyCodeToString(KeyCode keyCode)
    {
        var str = "";
        if (keyCode == KeyCode.Period || keyCode == KeyCode.KeypadPeriod)
            str = ".";
        else if (keyCode == KeyCode.KeypadDivide || keyCode == KeyCode.Slash)
            str = "/";
        else if (keyCode == KeyCode.KeypadMultiply || keyCode == KeyCode.Asterisk)
            str = "*";
        else if (keyCode == KeyCode.Minus || keyCode == KeyCode.KeypadMinus)
            str = "-";
        else if (keyCode == KeyCode.Plus || keyCode == KeyCode.KeypadPlus)
            str = "+";
        else if (keyCode == KeyCode.Equals || keyCode == KeyCode.KeypadEquals)
            str = "=";
        else if (keyCode == KeyCode.Exclaim)
            str = "!";
        else if (keyCode == KeyCode.DoubleQuote)
            str = "\"";
        else if (keyCode == KeyCode.Hash)
            str = "#";
        else if (keyCode == KeyCode.Dollar)
            str = "$";
        else if (keyCode == KeyCode.Percent)
            str = "%";
        else if (keyCode == KeyCode.Ampersand)
            str = "&";
        else if (keyCode == KeyCode.Quote)
            str = "'";
        else if (keyCode == KeyCode.LeftParen)
            str = "(";
        else if (keyCode == KeyCode.RightParen)
            str = ")";
        else if (keyCode == KeyCode.Comma)
            str = ",";
        else if (keyCode == KeyCode.Colon)
            str = ":";
        else if (keyCode == KeyCode.Semicolon)
            str = ";";
        else if (keyCode == KeyCode.Less)
            str = "<";
        else if (keyCode == KeyCode.Question)
            str = "?";
        else if (keyCode == KeyCode.At)
            str = "@";
        else if (keyCode == KeyCode.LeftBracket)
            str = "[";
        else if (keyCode == KeyCode.RightBracket)
            str = "]";
        else if (keyCode == KeyCode.LeftCurlyBracket)
            str = "{";
        else if (keyCode == KeyCode.RightCurlyBracket)
            str = "}";
        else if (keyCode == KeyCode.Backslash)
            str = "\\";
        else if (keyCode == KeyCode.Caret)
            str = "^";
        else if (keyCode == KeyCode.Underscore)
            str = "_";
        else if (keyCode == KeyCode.BackQuote)
            str = "`";
        else if (keyCode == KeyCode.Pipe)
            str = "|";
        else if (keyCode == KeyCode.Tilde)
            str = "~";
        else if (keyCode == KeyCode.Space)
            str = " ";
        else
        {
            str = keyCode.ToString();
            str = str.Substring(str.Length - 1);
        }
        
        return str;
    }
}
