using UnityEngine;

public class UserScripts : MonoBehaviour
{
    public User user;

    [System.Serializable]
    public class User
    {
        public int id;
        public string name = null;
        public string email;
        public string created_at;
        public string updated_at;
        public string lastname = null;
        public string username;
        public int background_id;
        public int language_id;
        public string phone = null;
        public Currency[] user_currencies;

        public TokenScript.Token token;

        public User()
        {
            id = 0;
            email = null;
            created_at = null;
            updated_at = null;
            username = null;
            user_currencies = null;
        }

        public void setToken(TokenScript.Token token_)
        {
            token = token_;
        }

        public User(TokenScript.Token token_)
        {
            token = token_;
        }

        public User(int id, string email, string created_at, string updated_at, string username, Currency[] currencies, PuzzleScript.Puzzle[] puzzles, string name = null, string lastname = null, string phone = null)
        {
            this.id = id;
            this.email = email;
            this.created_at = created_at;
            this.updated_at = updated_at;
            this.username = username;
            this.user_currencies = new Currency[user_currencies.Length];
            for (int i = 0; i < user_currencies.Length; i++)
            {
                this.user_currencies[i] = new Currency(user_currencies[i].id, user_currencies[i].count);
            }
            if (name != null)
                this.name = name;

            if (lastname != null)
                this.lastname = lastname;

            if (phone != null)
                this.phone = phone;
        }

        public int getCurrency(int id)
        {
            if (user_currencies == null)
                return 0;

            for (int i = 0; i < user_currencies.Length; i++)
            {
                if (user_currencies[i].currency_id == id)
                {
                    return user_currencies[i].count;
                }
            }
            return 0;
        }

        public void setCurrency(int id, int count)
        {
            for (int i = 0; i < user_currencies.Length; i++)
            {
                if (user_currencies[i].currency_id == id)
                {
                    user_currencies[i].count = count;
                }
            }
        }
    }

    [System.Serializable]
    public class Currency
    {
        public int id;
        public int count;
        public string created_at;
        public string updated_at;
        public string name;
        public int user_id;
        public int currency_id;
        public string description;
        public float value;

        public Currency()
        {
            count = 0;
            id = 0;
            created_at = "";
            updated_at = "";
            name = "";
            description = "";
            value = 0;
        }

        public Currency(int id, int count)
        {
            this.count = count;
            this.id = id;
        }
    }

    [System.Serializable]
    public class Currencies
    {
        public Currency[] user_currencies;

        public Currencies()
        {
            user_currencies = null;
        }

        public Currencies(Currency[] arr)
        {
            user_currencies = new Currency[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                user_currencies[i] = new Currency(arr[i].id, arr[i].count);
            }
        }

        public Currencies(Currencies other)
        {
            user_currencies = new Currency[other.user_currencies.Length];
            for (int i = 0; i < other.user_currencies.Length; i++)
            {
                user_currencies[i] = new Currency(other.user_currencies[i].id, other.user_currencies[i].count);
            }
        }
    }
}
