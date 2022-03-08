using UnityEngine;

public class ResponsePuzzle : MonoBehaviour
{
    public class PuzzleResponse
    {
        public PuzzleScript.Puzzle puzzle;
    }

    [System.Serializable]
    public class ActivePuzzle
    {
        public int time_spent;
    }

    [System.Serializable]
    public class UserPuzzle
    {
        public ActivePuzzle active_puzzle;
        public int id;
    }

    public class currentPuzzle
    {
        public UserPuzzle userPuzzle;
    }
}
