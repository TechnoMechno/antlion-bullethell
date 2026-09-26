using UnityEngine;

namespace AntLion.Core
{
    // Builds a closed oval EdgeCollider2D that keeps things inside the arena.
    [RequireComponent(typeof(EdgeCollider2D))]
    public class ArenaBounds : MonoBehaviour
    {
        [SerializeField] private float width = 16f;
        [SerializeField] private float height = 9.9f;
        [SerializeField, Range(8, 128)] private int segments = 48;

        // Runs in the editor whenever a value changes in the Inspector.
        private void OnValidate()
        {
            var edge = GetComponent<EdgeCollider2D>();
            var points = new Vector2[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                points[i] = new Vector2(Mathf.Cos(angle) * width * 0.5f, Mathf.Sin(angle) * height * 0.5f);
            }
            edge.points = points; // last point equals the first, so the loop is closed
        }
    }
}
