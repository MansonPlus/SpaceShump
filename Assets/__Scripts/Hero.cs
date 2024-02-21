using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; } // Singleton property

    [Header("Inscribed")]
    // This fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;

    [Header("Dynamic")] [Range(0, 4)] [SerializeField]
    public float _shieldLevel = 4;
    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;
    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    public event WeaponFireDelegate fireEvent;

    void Awake() {
        if (S == null) {
            S = this; // Set the Singleton only if it's null
        } else {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }
        // fireEvent += TempFire;
    }

    void Update()
    {
        // Pull up information from the Input class
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);

        // Allow the ship to fire
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     TempFire();
        // }

        // Use the fireEvent to fire weapons when the SpaceBar is pressed.
        if (Input.GetAxis("Jump") == 1 && fireEvent != null) {
            fireEvent();
        }
    }

    // void TempFire() {
    //     GameObject projGO = Instantiate<GameObject>(projectilePrefab);
    //     projGO.transform.position = transform.position;
    //     Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
    //     // rigidB.velocity = Vector3.up * projectileSpeed;

    //     ProjectileHero proj = projGO.GetComponent<ProjectileHero>();
    //     proj.type = eWeaponType.blaster;
    //     float tSpeed = Main.GET_WEAPON_DEFINITION(proj.type).velocity;
    //     rigidB.velocity = Vector3.up * tSpeed;
    // }

    private void OnTriggerEnter(Collider other) {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        // Debug.Log("Shield trigger hit by: " + go.gameObject.name);

        // Make sure it's not the same triggering go as last time
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null) {
            shieldLevel--;
            Destroy(go);
        } else {
            Debug.LogWarning    ("Shield trigger hit by non-Enemy: " + go.name);
        }
    }

    public float shieldLevel {
        get { return ( _shieldLevel); }
        private set {
            _shieldLevel = Mathf.Min(value, 4);
            // If the shield is going to be set to less than zero
            if (value < 0) {
                Destroy(this.gameObject);
                Main.HERO_DIED();
            }
        }
    }
}
