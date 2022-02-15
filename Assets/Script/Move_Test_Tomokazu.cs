using UnityEngine;
using UnityEngine.InputSystem;

public class Move_Test_Tomokazu : MonoBehaviour
{
    Vector3 move;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Debug.Log("Fire");
        }
    }

    void Update()
    {
        const float Speed = 1f;
        transform.Translate(move * Speed * Time.deltaTime);
    }
}