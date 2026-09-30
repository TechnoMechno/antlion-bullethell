using UnityEngine;
using UnityEngine.Pool;

namespace AntLion.Projectiles
{
    // Hands out reusable projectiles. Nothing may Instantiate or Destroy a bullet at runtime:
    // the boss's patterns get dense enough that allocating mid-fight would stutter.
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private Projectile prefab;
        [SerializeField] private int prewarmCount = 200;

        public int PrewarmCount => prewarmCount;

        private ObjectPool<Projectile> pool;
        private Transform container;

        private void Awake()
        {
            // Keeps the pooled objects out of the scene root in the Hierarchy.
            container = new GameObject("Pooled Projectiles").transform;
            container.SetParent(transform, false);

            pool = new ObjectPool<Projectile>(
                createFunc: CreateProjectile,
                actionOnGet: p => p.gameObject.SetActive(true),
                actionOnRelease: p => p.gameObject.SetActive(false),
                actionOnDestroy: p => {if (p!= null) Destroy(p.gameObject); } ,
                collectionCheck: true, // in the editor, throws if the same projectile is returned twice
                defaultCapacity: prewarmCount,
                maxSize: prewarmCount * 2);

            Prewarm();
        }

        private Projectile CreateProjectile()
        {
            Projectile projectile = Instantiate(prefab, container);
            projectile.SetPool(this); // so a projectile can return itself when it expires or hits
            return projectile;
        }

        // Creates every projectile up front, during load rather than mid-fight.
        private void Prewarm()
        {
            var warmed = new Projectile[prewarmCount];
            for (int i = 0; i < prewarmCount; i++) warmed[i] = pool.Get();
            for (int i = 0; i < prewarmCount; i++) pool.Release(warmed[i]);
        }

        // Returns an inactive projectile, creating one if the pool has run dry.
        // The caller then calls Launch() to place, aim and activate it.
        public Projectile Get()
        {
            return pool.Get();
        }

        public void Return(Projectile projectile)
        {
            pool.Release(projectile);
        }
    }
}
