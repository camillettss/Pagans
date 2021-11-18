using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [SerializeField] Text text;
    float timer = 2f;

    public void SetText(string txt)
    {
        text.text = txt;
    }

    public void HandleUpdate(NotificationsUI caller)
    {
        if (timer <= 0f)
        {
            StartCoroutine(goAway());
            caller.OnNotificationDeath();
        }

        else
        {
            timer -= Time.deltaTime;
        }
    }

    IEnumerator goAway()
    {
        GetComponent<Animator>().SetTrigger("destroy");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
