using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public int MaxHealth = 10;
    private int CurrentHealth;

    public int AttackDamage = 2;
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage()
    {
        
    }

    private void OnDeath()
    {
        
    }

    private void OnDamaged()
    {
        
    }
}
