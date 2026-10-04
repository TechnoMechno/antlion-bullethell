using UnityEngine;

namespace AntLion.Player
{
    // Tunable player numbers (move speed, max HP, bullet speed and damage).
    [CreateAssetMenu(menuName = "AntLion/Stats/Player Stats", fileName = "Stats_Player")]
    public class PlayerStats : ScriptableObject
    {
        [SerializeField] private int bulletDamage = 3;
        public int BulletDamage => bulletDamage;

        [SerializeField] private float bulletSpeed = 10;
        public float BulletSpeed => bulletSpeed;
    }
}
