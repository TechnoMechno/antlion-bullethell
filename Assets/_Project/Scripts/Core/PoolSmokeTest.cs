using AntLion.Projectiles;
using UnityEngine;

namespace AntLion.Core
{
    // TEMPORARY: verifies pooling, lifetime return and damage. Delete after checking the Console.
    public class PoolSmokeTest : MonoBehaviour
    {
        [SerializeField] private ProjectilePool pool;
        [SerializeField] private Health target;
        [SerializeField] private int bossProjectileLayer = 6;

        private Projectile shot;
        private float t;

        private void Start()
        {
            Debug.Log($"[SmokeTest] pooled children at start: {pool.transform.GetChild(0).childCount} (expect {pool.PrewarmCount})");
            Debug.Log($"[SmokeTest] target HP {target.Current}/{target.MaxHealth}");

            // Fire one bullet at the target from 1.5 units away.
            Vector2 targetPos = target.transform.position;
            Vector2 start = targetPos + Vector2.up * 1.5f;
            shot = pool.Get();
            shot.Launch(start, (targetPos - start).normalized, bossProjectileLayer);
            Debug.Log($"[SmokeTest] launched from {start} toward {targetPos}");
        }

        private void Update()
        {
            t += Time.deltaTime;
            if (t > 1f && shot != null)
            {
                Debug.Log($"[SmokeTest] after 1s: target HP {target.Current}/{target.MaxHealth}, bullet active {shot.gameObject.activeSelf}");
                shot = null;
            }
        }
    }
}
