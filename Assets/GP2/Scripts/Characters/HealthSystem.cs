using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private Rigidbody2D RB;
    private SpriteRenderer SpriteRenderer;
    
    public int MaxHealth = 10;
    private int CurrentHealth;

    public float DamagedCooldown;
    private float CurrentDamagedCooldown;
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        CurrentDamagedCooldown = DamagedCooldown;

        RB = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        
        GameObject.FindGameObjectWithTag("DungeonGenerator").GetComponent<DungeonGenerator>().OnGeneration.AddListener(OnDeath);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDamaged();
    }

    // todo clean logic up
    private void UpdateDamaged()
    {
        if (CurrentDamagedCooldown < 0)
        {
            SpriteRenderer.color = Color.white;
            return;
        }

        CurrentDamagedCooldown -= Time.deltaTime;
    }

    public void ApplyDamage(int damage, Vector2 direction)
    {
        CurrentHealth -= damage;
        print(CurrentHealth);
        if (CurrentHealth < 0)
        {
            OnDeath();
            return;
        }
        
        OnDamaged(damage, direction);
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }

    // todo shit performance
    /* Apply some knockback, maybe some damage related FX.*/
    private void OnDamaged(int damage, Vector2 direction)
    {
        gameObject.GetComponent<AIPath>().Move(0.5f * direction * damage);
        CurrentDamagedCooldown = DamagedCooldown;
        SpriteRenderer.color = Color.red;
        //gameObject.GetComponent<IAstarAI>().FinalizeMovement(gameObject.GetComponent<IAstarAI>().position, gameObject.GetComponent<IAstarAI>().rotation);
        //RB.AddForce(direction * damage, ForceMode2D.Impulse);
        print("I am being damaged");
    }
}
