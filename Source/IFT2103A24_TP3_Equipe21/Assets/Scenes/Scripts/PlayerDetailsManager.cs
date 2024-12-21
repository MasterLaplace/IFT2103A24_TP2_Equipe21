using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetailsManager : Singleton<PlayerDetailsManager>
{
    public Dictionary<string, PlayerDetail> playerDetails = new();

   protected override void Awake()
    {
        base.Awake();
    }

    public void AddMaterial(string name, Material material)
    {
        Debug.Log("Adding player material: " + name + " " + material.name);
        if (playerDetails.ContainsKey(name))
        {
            var detail = playerDetails[name];
            detail.material = material;
            playerDetails[name] = detail;
        }
        else
        {
            playerDetails[name] = new PlayerDetail
            {
                material = material,
                scale = 1f
            };
        }
    }

    public void AddScale(string name, float scale)
    {
        Debug.Log("Adding player scale: " + name + " " + scale);
        if (playerDetails.ContainsKey(name))
        {
            var detail = playerDetails[name];
            detail.scale = scale;
            playerDetails[name] = detail;
        }
        else
        {
            playerDetails[name] = new PlayerDetail
            {
                material = null,
                scale = scale
            };
        }
    }
}

public struct PlayerDetail
{
    public Material material;
    public float scale;
}
