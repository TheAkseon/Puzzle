using UnityEngine;

public class CategoryScript : MonoBehaviour
{
    public string image;
    public string Puzzles_count;
    public float priceFull;

    [System.Serializable]
    public class Category
    {
        public int id;
        public string created_at;
        public string updated_at;
        public string name;
        public string description;
        public string image;
        public int priceFull;
        public int puzzleCount;
        public int unlocked_count;
        public int discount_percentage;
        public int priceDiscount;

        public Category()
        {
            image = "";
            puzzleCount = 0;
            priceFull = 0;
        }

        public Category(string im, int number, int fp)
        {
            image = im;
            puzzleCount = number;
            priceFull = fp;
        }

        public Category(Category other)
        {
            image = other.image;
            puzzleCount = other.puzzleCount;
        }
    }
}
