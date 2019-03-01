using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outfit : MonoBehaviour
{
    // Outfit anims
    public AnimationClip[] attacks;
    // Outfit damage values
    public float[] attackDamage;
    // The mesh for the outfit
    public Mesh outfitMesh;
    // The outfit's material
    public Material outfitMaterial;
    // The player skin renderer to use(top, mid, bottom dependent)
    public SkinnedMeshRenderer outfitSkinRenderer;
    // Outfit type must be (punch, Kick, Misc)
    public string outfitType;
    // Colliders for this outfit's attack
    public Collider[] attackColliders; 
}
