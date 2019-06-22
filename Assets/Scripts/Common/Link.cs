using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link
{
    public static void Move(string url)
    {
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_WEBGL
        Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
#else
        Application.OpenURL(url);
#endif
    }
}
