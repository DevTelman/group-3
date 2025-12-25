using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float rotationSpeed = 100f;

        private void FixedUpdate()
        {
            float vertical = 0f;
            float horizontal = 0f;


            bool isRunning = Keyboard.current.wKey.isPressed;
            animator.SetBool("Run", isRunning);
            bool isRunBack = Keyboard.current.sKey.isPressed;
            animator.SetBool("RunBack", isRunBack);           

            if (Keyboard.current.aKey.isPressed) horizontal = -1;
            if (Keyboard.current.dKey.isPressed) horizontal = 1;

            // Rotate with A/D
            if (horizontal != 0)
            {
                rb.MoveRotation(
                    rb.rotation * Quaternion.Euler(0f, horizontal * rotationSpeed * Time.fixedDeltaTime, 0f));
            }
        }
    }
}