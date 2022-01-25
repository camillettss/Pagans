using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsController : MonoBehaviour
{
    [SerializeField] Cloud cloudPrefab;

    private void Start()
    {
        GenerateClouds();
    }

    private void FixedUpdate()
    {
        if(GameController.Instance.hours > 5 && GameController.Instance.hours < 19)
        {
            foreach (var cloud in Cloud.Instances)
                cloud.HandleUpdate();
        }
    }

    public void GenerateClouds()
    {
        var dir = new Vector3(RandomXYValue(), RandomXYValue());
        for (int i=1; i<Random.Range(3, 10); i++)
        {
            var cloud = Instantiate(cloudPrefab, transform);
            cloud.direction = dir;

            cloud.transform.position = new Vector3(Player.i.transform.position.x + Random.Range(-10, 10), Player.i.transform.position.y + Random.Range(-1, 10));
        }
    }

    public void NewDay()
    {
        // re-calculate clouds direction and speed (based on wind)
        var dir = new Vector3(RandomXYValue(), RandomXYValue());

        foreach (var cloud in Cloud.Instances)
            cloud.direction = dir;
    }

    int RandomXYValue()
    {
        var val = Random.Range(1, -1);
        if (val >= 0)
            return 1;
        else return -1;
    }
}
