using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyView : ViewBase
{
    public TextMeshProUGUI textMesh;

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
