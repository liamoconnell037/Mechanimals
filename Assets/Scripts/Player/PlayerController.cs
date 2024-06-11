using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;
    private Animator animator;
    private UnityEngine.Vector2 input;
    public event Action OnEncountered;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if(!isMoving) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if(input.x != 0) {  
                input.y = 0; // removes any diagonal movement
                animator.SetFloat("right", (int)input.x); // set animation side correctly
            }

            if(input != UnityEngine.Vector2.zero) {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                
                if(IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }
        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(UnityEngine.Vector3 targetPos) {
        isMoving = true;
        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) {
            transform.position = UnityEngine.Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(UnityEngine.Vector3 targetPos) {
        if(Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer) != null) {
            return false;
        } 
        return true;
    }

    private void CheckForEncounters() {
        if(Physics2D.OverlapCircle(transform.position, 0.3f, grassLayer) != null) {
            if(UnityEngine.Random.Range(1, 101) <= 15) {
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }
    }
}
