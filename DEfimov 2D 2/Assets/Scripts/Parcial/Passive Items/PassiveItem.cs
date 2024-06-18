using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;
    // Start is called before the first frame update

    protected virtual void ApplyModifier()
    {

    }
    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        ApplyModifier();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
