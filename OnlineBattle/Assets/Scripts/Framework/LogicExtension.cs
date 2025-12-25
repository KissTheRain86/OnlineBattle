using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogicExtension
{
    public static string _LogRed(this string format,params object[] args)
    {
        format = "<color=#FF0000>" + format + "</color>";
        return format;
    }
}
