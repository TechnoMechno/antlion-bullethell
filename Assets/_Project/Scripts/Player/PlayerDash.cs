using UnityEngine;
using AntLion.Core;

namespace AntLion.Player{
    public enum DashState{Ready , Dashing, Cooldown}

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Health health;
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float cooldown = 0.6f;

        public DashState State {get; private set;} = DashState.Ready;

        private Rigidbody2D rb;
        private Vector2 dashDirection;
        private float dashTimer;

        private void Awake(){
            rb = GetComponent<Rigidbody2D>();
        }

        public void Dash(Vector2 direction)
        {
            //Still Cooldownn
            if(State != DashState.Ready) return;

            //Find the dash direction
            dashDirection = direction.sqrMagnitude > 0.01f ? direction.normalized : movement.facingDirection;

            //Set the state to dashing
            State = DashState.Dashing;
            dashTimer = dashDuration;
            movement.OverriddenDirection = true;

            //Set invulnerable health to true
            health.SetInvulnerable(true);
        }

        private void FixedUpdate(){
            if(State == DashState.Ready){ return; }
            dashTimer -= Time.fixedDeltaTime;

            if(State == DashState.Dashing)
            {
                rb.linearVelocity = dashDirection * dashSpeed;

                //Check if the dash is over
                if(dashTimer <= 0f){
                    State = DashState.Cooldown;
                    dashTimer = cooldown;
                    movement.OverriddenDirection = false;
                    //Set invulnerable health to false
                    health.SetInvulnerable(false);
                }
            }
            else if (dashTimer <= 0f){
                //Check if the dash is over
                State = DashState.Ready;
            }
        }
    }
}

