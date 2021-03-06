﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class PlayerProjectile : MonoBehaviour, IPoolable
{
    [Tooltip("The speed of the projectile's movement.")]
    [SerializeField] private float _Speed = 0.0f;
    [Tooltip("The length of time before the bullet is culled.")]
    [SerializeField] private float _Lifetime = 0.0f;

    private Rigidbody2D _Rigidbody2D = null;
    private Transform _Transform = null;
    private Pool _Pool = null;

    private void Awake ()
    {
        Initialise ();
    }

    private void Initialise ()
    {
        _Rigidbody2D = GetComponent<Rigidbody2D> ();
        _Transform = GetComponent<Transform> ();
    }

    public void SetPool (Pool pool)
    {
        _Pool = pool;
    }

    private void Start ()
    {
        Setup ();
    }

    private void Setup ()
    {
        _Rigidbody2D.gravityScale = 0f;
        _Rigidbody2D.isKinematic = true;
        _Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnEnable ()
    {
        StartCoroutine (TimedCull (_Lifetime));
    }

    private void OnDisable ()
    {
        StopAllCoroutines ();
    }

    private void Update ()
    {
        Move ();
    }

    private void Move ()
    {
        _Transform.Translate (_Transform.up * Time.deltaTime * _Speed);
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (!other.CompareTag ("Player") && !other.CompareTag("Coin"))
            StartCoroutine (TimedCull (0.0f));
    }

    private IEnumerator TimedCull (float delay)
    {
        yield return new WaitForSeconds (delay);

        Cull ();
    }

    public void Cull ()
    {
        _Pool.ReturnToPool (this.gameObject);
    }
}
