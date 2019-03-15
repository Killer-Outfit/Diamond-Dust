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
    private bool outfit1MiscOn;
    private bool outfit1BotOn;

    private void Start()
    {
        outfit1TopOn = true;
        outfit1MiscOn = true;
        outfit1BotOn = true;
    }

    public void changeOutfit(string type)
    {
        if (type == "top")
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
        }else if(type == "misc")
        {
            if (outfit1MiscOn)
            {
                outfitModel.GetComponent<OutfitMenuModel>().changeOutfit(outfit2M);
                player.GetComponent<Player>().changeOutfit(outfit2M);
                outfit1MiscOn = false;
            }
            else
            {
                outfitModel.GetComponent<OutfitMenuModel>().changeOutfit(outfit1M);
                player.GetComponent<Player>().changeOutfit(outfit1M);
                outfit1MiscOn = true;
            }
        }else
        {
            if (outfit1BotOn)
            {
                outfitModel.GetComponent<OutfitMenuModel>().changeOutfit(outfit2Bot);
                player.GetComponent<Player>().changeOutfit(outfit2Bot);
                outfit1BotOn = false;
            }
            else
            {
                outfitModel.GetComponent<OutfitMenuModel>().changeOutfit(outfit1Bot);
                player.GetComponent<Player>().changeOutfit(outfit1Bot);
                outfit1BotOn = true;
            }
        }
    }
}
