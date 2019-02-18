using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outfit : MonoBehaviour
{
    public AnimationClip[] attacks;
    public float[] attackDamage;
    public Mesh outfitMesh;
    public Material outfitMaterial;
    public SkinnedMeshRenderer outfitSkinRenderer;
    public string outfitType;
    public Collider[] attackColliders;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AnimationClip getAnimClip(int animClipIndex)
    {
        return attacks[animClipIndex];
    }

}
