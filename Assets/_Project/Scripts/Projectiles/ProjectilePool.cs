using UnityEngine;

namespace AntLion.Projectiles
{
    // Step 1: pre-creates projectiles and hands them out. Never Instantiate/Destroy bullets at runtime.
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private Projectile prefab;
        [SerializeField] private int prewarmCount = 200;

        public int PrewarmCount => prewarmCount;

        public Projectile Get()
        {
            throw new System.NotImplementedException();
        }

        public void Return(Projectile projectile)
        {
        }
    }
}
