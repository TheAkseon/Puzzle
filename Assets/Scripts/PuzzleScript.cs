using UnityEngine;

public class PuzzleScript : MonoBehaviour
{
    public string image;
    public string Puzzles_count;

    [System.Serializable]
    public class Puzzle
    {
        public int id;
        public int category_id;
        public string created_at;
        public string updated_at;
        public string name;
        public string image;
        public string description;
        public bool is_unlocked;
        public float price;
        public string prices = null;
        public float category_price;
        public int percentage_completed;
        public bool is_active;
        public bool is_started;

        public Puzzle()
        {
            this.created_at = "";
            this.updated_at = "";
            this.image = "";
            this.description = "";
            this.is_unlocked = false;
            this.price = 0;
            this.category_price = 0;
            this.percentage_completed = 0;
            this.is_active = false;
        }

        public Puzzle(string created_at, string updated_at, string image, string description, bool is_unlocked, float price, float category_price, int percentage_completed, bool is_active, bool is_started)
        {
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.image = image;
            this.description = description;
            this.is_unlocked = is_unlocked;
            this.price = price;
            this.category_price = category_price;
            this.percentage_completed = percentage_completed;
            this.is_active = is_active;
        }

        public Puzzle(Puzzle other)
        {
            this.created_at = other.created_at;
            this.updated_at = other.updated_at;
            this.image = other.image;
            this.description = other.description;
            this.is_unlocked = other.is_unlocked;
            this.price = other.price;
            this.category_price = other.category_price;
            this.percentage_completed = other.percentage_completed;
            this.is_active = other.is_active;
        }
    }
}
