using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {
    public float moveDistance;
    float speed = 0.08f;

    Vector2 startPos;
    public Vector2 moveDirection;

    // Start is called before the first frame update
    void Start() {
        startPos = transform.position;
        moveDirection = moveDirection.normalized;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if ((transform.position.x > startPos.x + moveDistance) ||
            (transform.position.y > startPos.y + moveDistance) ||
            (transform.position.x < startPos.x - moveDistance) ||
            (transform.position.y < startPos.y - moveDistance)) {
            moveDirection *= -1;
        }
        transform.Translate(moveDirection * speed);
    }
}
