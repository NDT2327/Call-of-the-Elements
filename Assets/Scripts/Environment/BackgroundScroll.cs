using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        //calculate distance background move based on cam movement
        float distance = cam.transform.position.x * parallaxEffect; //0 = move with cam || 1 = won;t  move || o.5 = half
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
