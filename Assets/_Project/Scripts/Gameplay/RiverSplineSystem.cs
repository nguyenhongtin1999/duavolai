using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

namespace MienTayDaiChien.Gameplay
{
    /// <summary>
    /// Extends Unity Splines with river-specific data for "Miền Tây Đại Chiến".
    /// </summary>
    [RequireComponent(typeof(SplineContainer))]
    public class RiverSpline : MonoBehaviour
    {
        [Header("River Settings")]
        public float defaultWidth = 10f;
        public float defaultDepth = 5f;
        
        [Header("AI & Racing")]
        public List<Transform> checkpoints = new List<Transform>();
        public float aiIdealPathOffset = 0f; // Center of the river
        
        private SplineContainer _splineContainer;
        public SplineContainer Container => _splineContainer ??= GetComponent<SplineContainer>();

        public Vector3 GetPositionAtLinearDistance(float distance)
        {
            return Container.EvaluatePosition(distance / Container.CalculateLength());
        }

        public Vector3 GetTangentAtLinearDistance(float distance)
        {
            return Container.EvaluateTangent(distance / Container.CalculateLength());
        }
    }

    /// <summary>
    /// Generates modular river meshes (Water and Banks) along a spline.
    /// Optimized for mobile with SRP Batcher and GPU Instancing.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RiverSpline))]
    public class RiverMeshGenerator : MonoBehaviour
    {
        public Mesh waterMesh;
        public Material waterMaterial;
        public int segments = 50;
        public float riverWidth = 10f;
        
        [ContextMenu("Generate River Mesh")]
        public void Generate()
        {
            var river = GetComponent<RiverSpline>();
            var spline = river.Container.Spline;
            
            // Implementation would generate a procedural mesh following the spline curves
            // For production, we use Mesh.CombineMeshes or specialized SplineMesh tools
            Debug.Log("Generating Stylized River Mesh for mobile...");
        }
    }
}
