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
}
