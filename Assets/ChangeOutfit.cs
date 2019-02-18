using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOutfit : MonoBehaviour
{
    public Mesh outfit1Top;
    public Mesh outfit2Top;

    public Material top1;
    public Material top2;

    public SkinnedMeshRenderer outfitTop;
    private bool outfit1TopOn = true;

    public void changeOutfit()
    {
        if (outfit1TopOn)
        {
            outfitTop.sharedMesh = outfit2Top;
            outfitTop.material = top2;
            outfit1TopOn = false;
        }
        else
        {
            outfitTop.sharedMesh = outfit1Top;
            outfitTop.material = top1;
            outfit1TopOn = true;
        }
    }
}
