using UnityEngine;

namespace AntLion.Boss
{
    // Hold fire for a moment. This is the player's window to shoot back safely. The boss keeps circling, slowly.
    public class RecoverState : BossState
    {
        private float timer;

        public RecoverState(BossBrain brain) : base(brain) { }

        public override string Name => "Recover";

        public override void Enter()
        {
            timer = 0f;
        }

        public override void Tick()
        {
            timer += Time.deltaTime;
            if (timer >= brain.RecoverTime) brain.ChangeState(brain.Attacking);
        }
    }
}
