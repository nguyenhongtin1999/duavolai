using UnityEngine;

namespace MienTayDaiChien.Gameplay
{
    [CreateAssetMenu(menuName = "MienTayDaiChien/Gameplay/Race Rules Config", fileName = "RaceRulesConfig")]
    public class RaceRulesConfig : ScriptableObject
    {
        public int totalLaps = 3;
        public float countdownDuration = 3f;
        [Range(-1f, 1f)] public float wrongWayDotThreshold = -0.5f;
    }
}
