using UnityEngine;

public class PuzzleCompleted : MonoBehaviour
{
    [System.Serializable]
    public class PuzzleCompletedResult
    {
        public string message;
        public AchievmentsScripts.UserAcheivment timedAchievment;
        public BuyPuzzles.Currency[] currencies;
        public AchievmentsScripts.GameAchievment[] achievments;
    }
}
