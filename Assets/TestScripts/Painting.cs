using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public Transform LineParent;
    public SpriteRenderer SpriteRend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        SpriteRend.gameObject.SetActive(true);
    }

    public void Hide()
    {
        SpriteRend.gameObject.SetActive(false);
    }

    public void ClearLines()
    {
        for (int i = LineParent.childCount - 1; i >= 0; i--)
            Destroy(LineParent.GetChild(i).gameObject);
    }

    public void SetSprite(Sprite sprite)
    {
        SpriteRend.sprite = sprite;
        SpriteRend.gameObject.SetActive(true);
    }
}
