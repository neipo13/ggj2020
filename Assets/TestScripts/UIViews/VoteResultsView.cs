using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoteResultsView : ViewBase
{
    public TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
