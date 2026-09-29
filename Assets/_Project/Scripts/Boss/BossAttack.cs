using System.Collections;
using AntLion.Boss.Patterns;
using UnityEngine;

namespace AntLion.Boss
{
    // Plays whichever AttackPattern it's told to. Makes no decisions.
    public class BossAttack : MonoBehaviour
    {
        public bool IsAttacking { get; private set; }   // brain polls this to know when a pattern is done

        private AttackContext context;
        private Coroutine running;

        // BossController, once.
        public void Init(AttackContext attackContext)
        {
            context = attackContext;
        }

        // Brain: start this pattern. Replaces any pattern still running, so only one plays at a time.
        public void Run(AttackPattern pattern)
        {
            Stop();
            if (pattern == null)
            {
                Debug.LogWarning("BossAttack.Run was given no pattern. Is the boss's pattern list empty?", this);
                return;
            }

            IsAttacking = true;
            running = StartCoroutine(Play(pattern));
        }

        // Brain: interrupt (death, stagger, phase change). Bullets already in the air keep flying.
        public void Stop()
        {
            if (running != null) StopCoroutine(running);
            running = null;
            IsAttacking = false;
        }

        // Coroutines die with the GameObject; this keeps IsAttacking honest so the brain never waits forever.
        private void OnDisable()
        {
            Stop();
        }

        private IEnumerator Play(AttackPattern pattern)
        {
            yield return pattern.Execute(context);
            running = null;
            IsAttacking = false;
        }
    }
}
