using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutfitMenuModel : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeOutfit(outfit newOutfit)
    { 
        newOutfit.outfitMenuSkinRenderer.sharedMesh = newOutfit.outfitMesh;
        newOutfit.outfitMenuSkinRenderer.material = newOutfit.outfitMaterial;
    }
}
