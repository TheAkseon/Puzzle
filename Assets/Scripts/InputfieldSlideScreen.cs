using UnityEngine;

public class InputfieldSlideScreen : MonoBehaviour
{
    public bool InputFieldActive = false;
    private RectTransform tr;
    private float speed = 5000;
    private Vector2[] defPos = new Vector2[2];
    private Vector2[] target = new Vector2[2];

    private void Start()
    {
        tr = transform.GetComponent<RectTransform>();
        defPos[0] = tr.offsetMin;
        defPos[1] = tr.offsetMax;
        ResetPos();
    }

    private void LateUpdate()
    {
        if (InputFieldActive)
        {
            float height = TouchScreenKeyboard.area.y / Screen.height * tr.offsetMax.y;
            target[0] = new Vector2(defPos[0].x, defPos[0].y - height);
            target[1] = new Vector2(defPos[1].x, height);
        }
        else
        {
            ResetPos();
        }
        InputFieldActive = false;
    }

    private void ResetPos()
    {
        target[0] = defPos[0];
        target[1] = defPos[1];
    }

    private void Update()
    {
        tr.offsetMin = Vector2.MoveTowards(tr.offsetMin, target[0], Time.deltaTime * speed);
        tr.offsetMax = Vector2.MoveTowards(tr.offsetMax, target[1], Time.deltaTime * speed);
    }
}
