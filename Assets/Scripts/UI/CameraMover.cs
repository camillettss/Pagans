using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
=======
public enum CameraMovingType
{
    Static,
    Lerp,
    Threshold
}

>>>>>>> Stashed changes
public class CameraMover : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float threshold;
<<<<<<< Updated upstream
=======
    [SerializeField] float speed = 4f;

    [SerializeField] CameraMovingType type;
>>>>>>> Stashed changes

    Vector3 target;

    private void Update()
    {
<<<<<<< Updated upstream
        
=======
        if (type == CameraMovingType.Static)
            transform.position = player.position;

        else if (type == CameraMovingType.Lerp)
            transform.position = Vector3.Lerp(transform.position, player.position, speed * Time.deltaTime);

        else if (type == CameraMovingType.Threshold)
            transform.position = player.position; // missing!
>>>>>>> Stashed changes
    }
}
