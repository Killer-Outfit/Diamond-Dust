using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOutfit : MonoBehaviour
{
    public outfit outfit1;
    public outfit outfit2;
    public outfit outfit1Bot;
    public outfit outfit2Bot;
    public outfit outfit1M;
    public outfit outfit2M;
    public GameObject player;
    public GameObject outfitModel;

    private bool outfit1TopOn;

    private void Start()
    {
        outfit1TopOn = true;
    }

    public void changeOutfit()
    {
        if (outfit1TopOn)
        {
            outfitModel.GetComponent<OutfitMenuModel>().changeOutfit(outfit2);
            player.GetComponent<Player>().changeOutfit(outfit2);
            outfit1TopOn = false;
        }
        else
        {
            outfitModel.GetComponent<OutfitMenuModel>().changeOutfit(outfit1);
            player.GetComponent<Player>().changeOutfit(outfit1);
            outfit1TopOn = true;
        }
    }
}
