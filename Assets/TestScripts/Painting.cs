using System.Collections.Generic;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public Transform LineParent;
    public SpriteRenderer SpriteRend;

    public MeshRenderer CanvasMesh;
    public Camera RenderingCamera;

    public List<RenderTexture> renderTextures;
    public List<Material> renderTextureMaterials;


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

    //public void ClearLines()
    //{
    //    for (int i = LineParent.childCount - 1; i >= 0; i--)
    //        Destroy(LineParent.GetChild(i).gameObject);
    //}

    public void NewPainting(int playerIdx)
    {
        // Set the current player up with a new render texture to draw to
        // we need to keep these around for the voting phase

        var drawPad = renderTextures[playerIdx];
        RenderingCamera.targetTexture = drawPad;
        CanvasMesh.material = renderTextureMaterials[playerIdx];
    }

    public void SetSprite(Sprite sprite)
    {
        SpriteRend.sprite = sprite;
        SpriteRend.gameObject.SetActive(true);
    }
}
