using UnityEngine;

public class AchievmentsScripts : MonoBehaviour
{
    [System.Serializable]
    public class Achievment
    {
        public CupType[] types;
        public PieceCountClass pieceCount;
    }

    [System.Serializable]
    public class CupType
    {
        public int id;
        public string name;
        public int time;
    }

    [System.Serializable]
    public class PieceCountClass
    {
        public int id;
        public int piece_count;
    }

    [System.Serializable]
    public class AchievmentList
    {
        public Achievment[] timedAchievments;
        public int[] getTime(int count)
        {
            for (int i = 0; i < timedAchievments.Length; i++)
            {
                if (timedAchievments[i].pieceCount.piece_count == count)
                {
                    int[] times = new int[3];
                    times[0] = timedAchievments[i].types[0].time;
                    times[1] = timedAchievments[i].types[1].time;
                    times[2] = timedAchievments[i].types[2].time;
                    return times;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class UserAcheivment
    {
        public int type_id;
        public int puzzle_id;
    }

    [System.Serializable]
    public class UserAchievmentList
    {
        public UserAcheivment[] timedAchievments;
    }

    [System.Serializable]
    public class timedAchievment
    {
        public int type_id;
        public int puzzle_id;
        public int piece_count_id;
    }

    [System.Serializable]
    public class timedAchievmentsList
    {
        public timedAchievment[] timedAchievments;
    }

    [System.Serializable]
    public class timeAchievmentType
    {
        public int id;
        public string name;
    }

    [System.Serializable]
    public class PieceCount
    {
        public int id;
        public int piece_count;
    }

    [System.Serializable]
    public class timedAchievmentPos
    {
        public timeAchievmentType[] types;
        public PieceCount pieceCount;
    }

    [System.Serializable]
    public class timedAchievmentList
    {
        public timedAchievmentPos[] timedAchievments;
    }

    // GAME ACHIEVMENTS
    [System.Serializable]
    public class GameAchievment
    {
        public int id;
        public string created_at;
        public string updated_at;
        public string name;
        public string description;
        public string image;
        public string acquired_text;
        public int category_id;
        public int type_id;
        public int count;
        public bool isUnlocked;
    }
}

