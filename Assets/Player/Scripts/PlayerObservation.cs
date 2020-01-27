using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObservation : MonoBehaviour
{
    public Transform viewPoint;

    // Update is called once per frame
    void Update()
    {
        InteractRaycast();
    }

    void InteractRaycast()
    {
        Vector2 playerPosition = viewPoint.position;
        Vector2 forwardDirection = viewPoint.up;

        RaycastHit2D hit = Physics2D.Raycast(playerPosition, forwardDirection);

        if (hit.collider != null) {
            Debug.Log(hit.collider.gameObject.tag);
            Debug.Log(hit.point);
            Debug.Log(hit.distance);
        }
    }
}
