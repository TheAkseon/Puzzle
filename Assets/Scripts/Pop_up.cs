using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlesComparer : IComparer<PuzzleScript.Puzzle>
{
    public int Compare(PuzzleScript.Puzzle x, PuzzleScript.Puzzle y)
    {
        return x.percentage_completed > y.percentage_completed || x.percentage_completed == y.percentage_completed && x.id < y.id ? -1 : 1;
    }
}

public class Pop_up : MonoBehaviour
{
    public class PopUp
    {
        public int ImageIndex = 0;
        public string CategoryId;
        public PuzzlesScript.Puzzles PuzzlesList;
        private static Texture2D loaded;
        private bool firstDraw = true;
        private static Texture cTex;

        public void DrawButtons(PuzzleScript.Puzzle v)
        {
            //GameObject.Find("PuzzleInfo").transform.Find("lockPuzzle").gameObject.SetActive(!v.is_unlocked);
            if (firstDraw)
            {
                GameObject.Find("PuzzleInfo").transform.Find("lockPuzzle").GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
                firstDraw = false;
            }
            GameObject.Find("PuzzleInfo").transform.Find("lockPuzzle").GetComponent<Image>().CrossFadeAlpha(v.is_unlocked ? 0 : 1, 0.5f, false);
            GameObject.Find("PuzzleInfo").transform.Find("CompliteText").GetComponent<Text>().text = v.percentage_completed > 0 && v.percentage_completed < 100 ? v.percentage_completed + "%" : string.Empty;
            if (v.is_unlocked && (!v.is_started || v.percentage_completed == 0))
            {
                GameObject.Find("PuzzleInfo").transform.Find("Continue").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("PlayAgain").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BuyThisPuzzle").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BuyAllPuzzles").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("Buy").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("OR").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BeginPlaying").gameObject.SetActive(true);
                BeginPlaying.idPuzzle = v.id;
                BeginPlaying.idCategory = CategoryId;

            }
            else if (v.is_unlocked && (v.is_started || (v.percentage_completed < 100 && v.percentage_completed > 0)))
            {
                GameObject.Find("PuzzleInfo").transform.Find("Continue").gameObject.SetActive(true);
                GameObject.Find("PuzzleInfo").transform.Find("PlayAgain").gameObject.SetActive(true);
                BeginPlaying.idPuzzle = v.id;
                ContinueScript.idPuzzle = v.id;
                BeginPlaying.idCategory = CategoryId;
                ContinueScript.idCategory = CategoryId;
                GameObject.Find("PuzzleInfo").transform.Find("BuyThisPuzzle").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BuyAllPuzzles").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BeginPlaying").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("Buy").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("OR").gameObject.SetActive(false);

            }
            else if (v.is_unlocked && v.percentage_completed >= 100)
            {
                GameObject.Find("PuzzleInfo").transform.Find("Continue").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("PlayAgain").gameObject.SetActive(true);

                BeginPlaying.idPuzzle = v.id;
                BeginPlaying.idCategory = CategoryId;
                GameObject.Find("PuzzleInfo").transform.Find("BuyThisPuzzle").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BuyAllPuzzles").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BeginPlaying").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("Buy").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("OR").gameObject.SetActive(false);
            }
            else if (!v.is_unlocked)
            {
                bool categoryStat = BeginPlaying.categoryPrice > 0;
                GameObject.Find("PuzzleInfo").transform.Find("Continue").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("PlayAgain").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("BuyThisPuzzle").transform.Find("Price").transform.Find("Text").GetComponent<Text>().text = v.price.ToString();
                GameObject.Find("PuzzleInfo").transform.Find("BuyThisPuzzle").gameObject.SetActive(true);
                GameObject.Find("PuzzleInfo").transform.Find("BuyAllPuzzles").transform.Find("Price").transform.Find("Text").GetComponent<Text>().text = BeginPlaying.categoryPrice.ToString();
                GameObject.Find("PuzzleInfo").transform.Find("BuyAllPuzzles").gameObject.SetActive(categoryStat);
                GameObject.Find("PuzzleInfo").transform.Find("Buy").gameObject.SetActive(true);
                GameObject.Find("PuzzleInfo").transform.Find("OR").gameObject.SetActive(categoryStat);
                GameObject.Find("PuzzleInfo").transform.Find("BeginPlaying").gameObject.SetActive(false);
                //GameObject.Find("PuzzleInfo").transform.Find("BuyThisPuzzle").transform.localPosition = new Vector2(categoryStat ? -544: 164.7f, -481.2f);
                //GameObject.Find("PuzzleInfo").transform.Find("Buy").transform.localPosition = new Vector2(categoryStat ? -706.7f: -12, -556.1f);
                BuyPuzzleScript.idPuzzle = v.id;
                BuyPuzzleScript.idCategory = CategoryId;
            }
            if (GameObject.Find("PuzzleInfo").transform.Find("BuyThisPuzzle").gameObject.activeInHierarchy)
            {
                Tutorial t = ((Tutorial)FindObjectOfType(typeof(Tutorial)));
                t.tList[0].selectBtn[0] = GameObject.Find("Coins");
                t.enabled = true;
            }
        }

        public void ParseJson(string json, int state)
        {
            PuzzlesList = JsonUtility.FromJson<PuzzlesScript.Puzzles>(json);
            PuzzlesList.puzzles.Sort(new PuzzlesComparer());
            BeginPlaying.Puzzles = PuzzlesList;
            BuyPuzzleScript.Puzzles = PuzzlesList;
            CategoryPopUp1.PuzzlesList = PuzzlesList;
            if (PuzzlesList.puzzles.Count > 0)
            {
                CategoryPopUp1.PopUpClassObject.PuzzlesList = PuzzlesList;
                ImageIndex = 0;
                BeginPlaying.PuzzleIndex = ImageIndex;
                GameObject.Find("PuzzleInfo").transform.Find("Buy").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("OR").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("CategoryName").GetComponent<Text>().text = CategoryName(PuzzlesScript.FindCategory(int.Parse(CategoryId)).name);
                PuzzleRender();
                InternetConnectionProblemScripts.setMethod(GameObject.Find("PuzzleInfo").GetComponent<BeginPlaying>().TryAgain);
                APIMethodsScript.sendRequest("get", "/api/puzzle/" + PuzzlesList.puzzles[BeginPlaying.PuzzleIndex].id, setImage);
                SetLoadedTexture();
                GameObject.Find("PuzzleInfo").GetComponent<CategoryPopUp1>().StartCoroutine(Wait());
            }
            else
            {
                Destroy(GameObject.Find("Loader"));
            }
        }

        public string CategoryName(string name)
        {
            var collectionLocalized = LocalizationManager.Instance.GetLocalizedValue("collection");
            return collectionLocalized + " «" + name + "»";
        }

        public void getPuzzles(string json, int state)
        {

        }

        public void setImage(string json, int status)
        {
            ResponsePuzzle.PuzzleResponse pz = JsonUtility.FromJson<ResponsePuzzle.PuzzleResponse>(json);
            ImagesScript.setTextureFromURL(pz.puzzle.image, GameObject.Find("Loader"), GameObject.Find("PuzzleInfo").transform.Find("Masking").transform.Find("PuzzlePreview").GetComponent<RawImage>(), pz.puzzle.id.ToString(), pz.puzzle.updated_at, pz.puzzle.category_id.ToString(), "big");
        }

        public void ShowCategoryPopUp(string id)
        {
            CategoryId = id;
            ImageIndex = 0;
            BeginPlaying.idCategory = CategoryId;
            BeginPlaying.photoPuzzleName = "";
            //InternetConnectionProblemScripts.setMethod(setImage);
            APIMethodsScript.sendRequest("get", "/api/puzzle?category_id=" + CategoryId, ParseJson);
            if (GameObject.Find("PuzzlesPanel") != null)
            {
                GameObject.Find("PuzzlesPanel").GetComponent<ScrollRect>().StopMovement();
                GameObject.Find("PuzzlesPanel").GetComponent<ScrollRect>().enabled = false;
            }
        }

        public void showPuzzleById(int ImageIndex1, PuzzlesScript.Puzzles P)
        {
            BeginPlaying.photoPuzzleName = "";
            ImageIndex = ImageIndex1;
            PuzzlesList = P;
            BeginPlaying.Puzzles = PuzzlesList;
            BeginPlaying.idCategory = GameObject.Find("puzzleInformation").GetComponent<puzzleInformationScripts>().puzzleInfo.idCategory.ToString();
            BuyPuzzleScript.Puzzles = PuzzlesList;
            CategoryPopUp1.PuzzlesList = PuzzlesList;
            BeginPlaying.PuzzleIndex = ImageIndex;
            BuyPuzzleScript.puzzleIndex = ImageIndex;
            CategoryPopUp1.PopUpClassObject.PuzzlesList = PuzzlesList;
            CategoryPopUp1.PopUpClassObject.PuzzlesList = PuzzlesList;
            if (UIManagerScript.GetActiveScene() == "puzzleInfo")
            {
                GameObject.Find("PuzzleInfo").transform.Find("Buy").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("OR").gameObject.SetActive(false);
                GameObject.Find("PuzzleInfo").transform.Find("CategoryName").GetComponent<Text>().text = CategoryName(PuzzlesScript.FindCategory(GameObject.Find("puzzleInformation").GetComponent<puzzleInformationScripts>().puzzleInfo.idCategory).name);
                PuzzleRender();
                InternetConnectionProblemScripts.setMethod(GameObject.Find("PuzzleInfo").GetComponent<BeginPlaying>().TryAgain);
                SetLoadedTexture();
                GameObject.Find("PuzzleInfo").GetComponent<CategoryPopUp1>().StartCoroutine(Wait());
            }
            if (P != null)
            {
                APIMethodsScript.sendRequest("get", "/api/puzzle/" + PuzzlesList.puzzles[ImageIndex].id, setImage);
            }
        }

        public void NextPuzzle(int v)
        {
            ImageIndex += v;
            UIManagerScript.StartLoader();
            PuzzleRender();
            APIMethodsScript.sendRequest("get", "/api/puzzle/" + PuzzlesList.puzzles[ImageIndex].id, setImage);
            SetLoadedTexture();
            GameObject.Find("PuzzleInfo").GetComponent<CategoryPopUp1>().StartCoroutine(Wait());
            BeginPlaying.PuzzleIndex = ImageIndex;
            BuyPuzzleScript.puzzleIndex = ImageIndex;
        }

        public void PuzzleRender()
        {
            if (PuzzlesList != null && PuzzlesList.puzzles.Count > ImageIndex)
            {
                GameObject.Find("PuzzleInfo").transform.Find("LeftArrow").gameObject.SetActive(ImageIndex != 0);
                GameObject.Find("PuzzleInfo").transform.Find("RightArrow").gameObject.SetActive(ImageIndex < CategoryPopUp1.PopUpClassObject.PuzzlesList.puzzles.Count - 1);
                GameObject.Find("PuzzleInfo").transform.Find("Title").transform.Find("Title").GetComponent<Text>().text = PuzzlesList.puzzles[ImageIndex].name;
                GameObject.Find("PuzzleInfo").transform.Find("Counter").transform.Find("Text").GetComponent<Text>().text = (ImageIndex + 1).ToString() + "/" + PuzzlesList.puzzles.Count;
                string d = PuzzlesList.puzzles[ImageIndex].description;
                var noDescriptionsLocalization = LocalizationManager.Instance.GetLocalizedValue("no_descriptions");
                d = d == string.Empty ? noDescriptionsLocalization : d;
                d = "\n" + d + "\n";
                GameObject.Find("PuzzleInfo").transform.Find("DescriptionBorder").transform.Find("DescriptionPanel").transform.Find("Wrapper").transform.Find("Description").GetComponent<Text>().text = d;
                DrawButtons(PuzzlesList.puzzles[ImageIndex]);
            }
        }

        private static IEnumerator Wait()
        {
            GameObject go = GameObject.Find("PuzzleInfo").transform.Find("Masking").gameObject;
            RawImage image = go.transform.Find("PuzzlePreview").GetComponent<RawImage>();
            Vector2 size = go.GetComponent<RectTransform>().sizeDelta;
            while (image.texture == loaded || image.texture == null)
            {
                yield return new WaitForEndOfFrame();
            }
            if (cTex != null)
            {
                Destroy(cTex);
                System.GC.Collect();
            }
            cTex = image.texture;
            resizeImage(image, size);
        }

        private static void SetLoadedTexture()
        {
            if (loaded == null)
            {
                loaded = (Texture2D)Resources.Load("Images/Interface/frame", typeof(Texture2D));
            }
            GameObject.Find("PuzzleInfo").transform.Find("Masking").transform.Find("PuzzlePreview").GetComponent<RawImage>().texture = loaded;
        }

        public static void resizeImage(RawImage image, Vector2 size)
        {
            image.SetNativeSize();
            float new_width = size.x;
            float new_height = new_width / image.texture.width * image.texture.height;
            if (new_height < size.y)
            {
                new_height = size.y;
                new_width = new_height / image.texture.height * image.texture.width;
                image.GetComponent<RectTransform>().localScale = new Vector2(new_height / image.texture.height, new_height / image.texture.height);
            }
            else
            {
                image.GetComponent<RectTransform>().localScale = new Vector2(new_width / image.texture.width, new_width / image.texture.width);
            }
            image.transform.localPosition = new Vector3(0, 0, 0);
            Destroy(GameObject.Find("Loader"));
        }
    }
}
