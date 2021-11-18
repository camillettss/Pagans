using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsUI : MonoBehaviour
{
    [SerializeField] Notification notificationPrefab;
    [SerializeField] int maxNotifications = 3;

    public static NotificationsUI i;

    List<string> queue = new List<string>();

    private void Awake()
    {
        i = this;
    }

    private void Update()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Notification>().HandleUpdate(this);
        }
    }

    public void OnNotificationDeath()
    {
        if(queue.Count > 0)
        {
            AddNotification(queue[0]);
            queue.RemoveAt(0);
        }
    }

    public void AddNotification(string text)
    {
        if(transform.childCount < maxNotifications)
        {
            var not = Instantiate(notificationPrefab, transform);
            not.SetText(text);
        }
        else
        {
            queue.Add(text);
        }
    }
}
