using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Inscribed")]
    public float rotationsPerSecond = 0.1f;

    [Header("Dynamic")]
    public float levelShown = 0;

    Material mat;
    
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Read the current shield level from the Hero Singleton
        int currentLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        // If this is different from levelShown...
        if (levelShown != currentLevel) {
            levelShown = currentLevel;
            // Adjust the texture offset to show different shield level
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);   
        }
        // Rotate the shield a bit every frame in a time-based way
        float rZ = -(rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
    }
}
