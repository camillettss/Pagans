using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public int enemyCount = 1;
    float WinDuration = 3f;

    public void onEnemyDie()
    {
        enemyCount -= 1;
        if(enemyCount <= 0)
        {
            StartCoroutine(Win());
        }
    }

    IEnumerator Win()
    {
        yield return new WaitForSeconds(WinDuration);
        GameController.Instance.EndBattle();
    }
}
