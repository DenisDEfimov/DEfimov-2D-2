using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;

    private void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    private void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        bool isMoving = distanceMoved > 0.001f;
        bool isShooting = Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space);

        SetAnimationState(isShooting ? AnimationState.Shoot : isMoving ? AnimationState.Move : AnimationState.Idle);
        lastPosition = transform.position;
    }

    private void SetAnimationState(AnimationState state)
    {
        animator.SetBool("isMoving", state == AnimationState.Move);
        animator.SetBool("isShooting", state == AnimationState.Shoot);
        animator.SetBool("isDead", state == AnimationState.Death);
    }

    private enum AnimationState { Idle, Move, Shoot, Death }
}
