using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public GameObject bullet;
    public Animator animator;
    private Rigidbody2D rb;
    public float life = 5f;
    public float destroyDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Destroy(bullet, life);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetTrigger("explode");
        Destroy(bullet, destroyDelay);
    }
}
