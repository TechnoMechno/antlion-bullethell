using UnityEngine;

namespace AntLion.Core
{
    // Builds a closed rounded-rectangle EdgeCollider2D that keeps things inside the arena.
    [RequireComponent(typeof(EdgeCollider2D))]
    public class ArenaBounds : MonoBehaviour
    {
        [SerializeField] private float width = 16f;
        [SerializeField] private float height = 9.9f;
        [Tooltip("Roundness of the corners in world units. 0 = sharp corners.")]
        [SerializeField, Min(0f)] private float cornerRadius = 1.5f;
        [Tooltip("Points used to draw each rounded corner.")]
        [SerializeField, Range(1, 32)] private int cornerSegments = 8;

        // Pulls a point back inside the shape (shrunk by `margin`) if it lies outside; points already inside are unchanged.
        public Vector2 ClampInside(Vector2 point, float margin)
        {
            float halfWidth = Mathf.Max(0.01f, width * 0.5f - margin);
            float halfHeight = Mathf.Max(0.01f, height * 0.5f - margin);
            float radius = Mathf.Min(cornerRadius, halfWidth, halfHeight);
            Vector2 local = transform.InverseTransformPoint(point);

            // The straight part of the shape: a smaller box whose corners are the centres of the rounded corners.
            Vector2 inner = new Vector2(halfWidth - radius, halfHeight - radius);
            Vector2 nearest = new Vector2(Mathf.Clamp(local.x, -inner.x, inner.x), Mathf.Clamp(local.y, -inner.y, inner.y));

            // Anything further than `radius` from that box is outside; pull it back onto the edge.
            Vector2 offset = local - nearest;
            if (offset.magnitude > radius) local = nearest + offset.normalized * radius;

            return transform.TransformPoint(local);
        }

        // Runs in the editor whenever a value changes in the Inspector.
        private void OnValidate()
        {
            var edge = GetComponent<EdgeCollider2D>();
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            float radius = Mathf.Min(cornerRadius, halfWidth, halfHeight);

            // Corner centres, going counter-clockwise from top-right. Each corner draws a quarter circle.
            Vector2[] centres =
            {
                new Vector2(halfWidth - radius, halfHeight - radius),
                new Vector2(-halfWidth + radius, halfHeight - radius),
                new Vector2(-halfWidth + radius, -halfHeight + radius),
                new Vector2(halfWidth - radius, -halfHeight + radius),
            };

            var points = new Vector2[4 * (cornerSegments + 1) + 1];
            int index = 0;
            for (int corner = 0; corner < 4; corner++)
            {
                for (int i = 0; i <= cornerSegments; i++)
                {
                    float angle = (corner + (float)i / cornerSegments) * Mathf.PI * 0.5f;
                    points[index++] = centres[corner] + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                }
            }
            points[index] = points[0]; // last point equals the first, so the loop is closed
            edge.points = points;
        }
    }
}
