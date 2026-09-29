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

        // Pulls a point back inside the oval (shrunk by `margin`) if it lies outside; points already inside are unchanged.
        public Vector2 ClampInside(Vector2 point, float margin)
        {
            float halfWidth = Mathf.Max(0.01f, width * 0.5f - margin);
            float halfHeight = Mathf.Max(0.01f, height * 0.5f - margin);
            Vector2 local = transform.InverseTransformPoint(point);

            // How far out the point is, where 1 is exactly on the shrunk oval's edge.
            float reach = Mathf.Sqrt((local.x * local.x) / (halfWidth * halfWidth) + (local.y * local.y) / (halfHeight * halfHeight));
            if (reach > 1f) local /= reach;

            return transform.TransformPoint(local);
        }

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
