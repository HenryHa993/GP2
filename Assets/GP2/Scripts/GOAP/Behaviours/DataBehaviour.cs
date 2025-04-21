using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBehaviour : MonoBehaviour
{
    public int FoodCount = 0;
    public float Hunger = 0f;

    // Update is called once per frame
    void Update()
    {
        Hunger += Time.deltaTime * 5f;
    }
}
