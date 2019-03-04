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
    // The outfit menu skin renderer to use(top, mid, bottom dependent)
    public SkinnedMeshRenderer outfitMenuSkinRenderer;
    // Outfit type must be (Top, Bot, Misc)
    public string outfitType;
    // Attack type must be (punch, Kick, misc)
    public string attackType;
    // Colliders for this outfit's attack
    public Collider[] attackColliders;
    // attack times each array is the wind up time, collision time and end time
    public float[] attack1Time;
    public float[] attack2Time;
    public float[] attack3Time;
    public float[] attack4Time;
    // Multiplier to change an attack animation
    public float[] animSpeedMultiplier;
    private float[,] attackTimes;
    void start()
    {
        attackTimes = new float[,]
        {
            {attack1Time[0], attack1Time[1], attack1Time[2] },
            {attack2Time[0], attack2Time[1], attack2Time[2] },
            {attack3Time[0], attack3Time[1], attack3Time[2] },
            {attack4Time[0], attack4Time[1], attack4Time[2] }
        };
        Debug.Log(attackTimes);
    }

    public float getTimeInterval(int list,int element)
    {
        if(list == 0)
        {
            return attack1Time[element];
        }else if (list == 1)
        {
            return attack2Time[element];
        }else if (list == 2)
        {
            return attack3Time[element];
        }
        else
        {
            return attack4Time[element];
        }
        
    }
}
