using Unity.VisualScripting;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed = 1f;
    private float backgroundWidth;
    private Transform[] backgrounds;
    void Start()
    {
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }

        SpriteRenderer sr = backgrounds[0].GetComponent<SpriteRenderer>();
        backgroundWidth = sr.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        foreach (Transform t in backgrounds)
        {
            if (t.position.x < -backgroundWidth)
            {
                t.position += Vector3.right * backgroundWidth * 2;
            }
        }
    }
}
