using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlay2D : MonoBehaviour
{
    public Transform player;
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody2D rb;

    private Vector2 direction;
    public void FixedUpdate()
    {
        direction = Vector2.up * variableJoystick.Vertical + Vector2.right * variableJoystick.Horizontal;
        //rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        moveCharacter(direction);
    }
    void moveCharacter(Vector2 direction)
    {
        player.Translate(direction * speed * Time.deltaTime);
    }
}
