using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextAnimator : MonoBehaviour
{

    static TextAnimator Instance;
    public float timeToAnimate;
    private TMP_Text textComp;

    private void Awake()
    {
        Instance = this;
        textComp = transform.GetComponent<TMP_Text>();
    }

    public IEnumerator animateText(string text, float time)
    {
        textComp.text = "";
        float t = time / text.Length;
        for (int i = 0; i < text.Length; i++)
        {
            textComp.text += text[i];
            yield return new WaitForSeconds(t);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
