using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace MienTayDaiChien.Gameplay
{
/// <summary>
    /// Authoritative race manager for ranking and progress tracking.
    /// Works for both Players and AI.
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [Header("Race Config")]
        public int totalLaps = 3;
        public RiverSpline riverSpline;
        
        private List<RaceProgress> _racers = new List<RaceProgress>();
        private List<RaceCheckpoint> _checkpoints = new List<RaceCheckpoint>();

        public List<RaceProgress> Rankings => _racers.OrderByDescending(r => r.Lap)
                                                     .ThenByDescending(r => r.LastCheckpointIndex)
                                                     .ThenByDescending(r => r.DistanceOnSpline)
                                                     .ToList();

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterRacer(RaceProgress racer)
        {
            if (!_racers.Contains(racer)) _racers.Add(racer);
        }

        public void RegisterCheckpoint(RaceCheckpoint cp)
        {
            if (!_checkpoints.Contains(cp)) _checkpoints.Add(cp);
            _checkpoints = _checkpoints.OrderBy(c => c.checkpointIndex).ToList();
        }

        public int GetCheckpointCount() => _checkpoints.Count;

        private void Update()
        {
            // Update Rankings for UI/Minimap every frame (optimized for mobile)
            // In a real multiplayer game, this might be synced at a lower tick rate
        }
        
        public bool IsWrongWay(Transform racerTransform, float t)
        {
            if (riverSpline == null) return false;
            Vector3 forward = racerTransform.forward;
            Vector3 tangent = riverSpline.Container.EvaluateTangent(t);
            return Vector3.Dot(forward, tangent) < -0.5f; // Threshold for wrong way
        }
        }
        }
