﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratorController : MonoBehaviour
{

    Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
        if (rb2d != null) {
            rb2d.velocity += (rb2d.velocity.normalized * 3gf);
            SoundManager.SwingSound();
            animator.SetTrigger("Boost");
        }
    }
}
