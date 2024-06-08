using UnityEngine;

public class PlayerSizeCalibration : MonoBehaviour
{
    public float minScale = 0.5f;
    public float maxScale = 2f;
    public float scaleStep = 0.1f;

    private Transform player;
    private Transform sizeReferenceObject;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        sizeReferenceObject = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        sizeReferenceObject.position = new Vector3(0f, 1f, 5f);
        sizeReferenceObject.localScale = Vector3.one;
    }

    void Update()
    {
        // Увеличение размера игрока
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePlayerScale(scaleStep);
        }
        // Уменьшение размера игрока
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePlayerScale(-scaleStep);
        }
        // Сброс размера игрока
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetPlayerScale();
        }
    }

    void ChangePlayerScale(float scaleChange)
    {
        float newScale = Mathf.Clamp(player.localScale.x + scaleChange, minScale, maxScale);
        player.localScale = Vector3.one * newScale;
    }

    void ResetPlayerScale()
    {
        player.localScale = Vector3.one;
    }
}