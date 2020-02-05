using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    public void Move(Vector2 movement, float angle)
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime);

        angle = angle * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}
