using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Enemy 2 Inscribed Fields")]
    public float lifetime = 10;
    // Enemy 2 uses a Sine wave to modify a 2-point linear interpolation
    [Tooltip("Determines how much the Sine wave will ease the interpolation")]
    public float sinEccentricity = 0.6f;
    public AnimationCurve rotCurve;

    [Header("Enemy_2 Private Fields")]
    [SerializeField] private float birthTime;

    private Quaternion baseRotation;

    [SerializeField] private Vector3 p0, p1;

    void Start()
    {
        // Pick any point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // Pick any point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // Possibly swap sides
        if (Random.value > 0.5f) {
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;

        // Set up the initial ship rotation
        transform.position = p0;
        transform.LookAt(p1, Vector3.back);
        baseRotation = transform.rotation;
    }

    public override void Move() {
        // Linear interpolations work based on a u value between 0 & 1
        float u = (Time.time - birthTime) / lifetime;

        if (u > 1) {
            Destroy(this.gameObject);
            return;
        }

        // Use the AnimationCurve to set the rotation about Y
        float shipRot = rotCurve.Evaluate(u) * 360;
        // if (p0.x > p1.x) shipRot = -shipRot;
        // transform.rotation = Quaternion.Euler(0, shipRot, 0);
        transform.rotation = baseRotation * Quaternion.Euler(-shipRot, 0, 0);

        // Adjust u by adding a U Curve based on a Sine wave
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        // Interpolate the two linear interpolation points
        pos = (1 - u) * p0 + u * p1;
    }

}
