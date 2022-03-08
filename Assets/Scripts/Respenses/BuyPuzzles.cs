using UnityEngine;

public class BuyPuzzles : MonoBehaviour
{

    [System.Serializable]
    public class BuyPuzzleClass
    {
        public int user_id;
        public int puzzle_id;
        public int is_unlocked;
        public string updated_at;
        public string created_at;
        public int id;
    }

    [System.Serializable]
    public class Currency
    {
        public int currency_id;
        public int count;
    }

    [System.Serializable]
    public class BuyPuzzleResponse
    {
        public BuyPuzzleClass[] puzzles;
        public string message;
        public Currency[] currencies;
        public AchievmentsScripts.GameAchievment[] achievments;
    }
}
