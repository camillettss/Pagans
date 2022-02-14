using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMovingType
{
    Static,
    Lerp,
    Threshold
}

public class CameraMover : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float threshold;
    [SerializeField] float speed = 4f;

    [SerializeField] CameraMovingType type;

    public static CameraMover i;

    Vector3 target;

    //Shake
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPos.x+x, originalPos.y+y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;

    }

    private void Awake()
    {
        i = this;
    }

    public void HandleUpdate()
    {
        if (type == CameraMovingType.Static)
            transform.position = player.position;

        else if (type == CameraMovingType.Lerp)
            transform.position = Vector3.Lerp(transform.position, player.position, speed * Time.deltaTime);

        else if (type == CameraMovingType.Threshold)
        {
            transform.position = player.transform.position;
        }
    }
}
