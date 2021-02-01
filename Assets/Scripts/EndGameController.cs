using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Author: Tomas
/// Controls Players endgame. Changes the position to one of two small rooms.
/// </summary>
public class EndGameController : MonoBehaviour
{
    public float timeOut = 10f;
    public GameObject badPosition;
    public GameObject goodPosition;
    public GameObject VRplayer;

    void Start()
    {
        EventManager.instance.OnLoseGame += LoseGameEvent;
        EventManager.instance.OnWinGame += WinGameEvent;
    }

    public void WinGameEvent()
    {
        StartCoroutine(GameOverGood());
    }

    public void LoseGameEvent()
    {
        StartCoroutine(GameOverBad());
    }
    /// <summary>
    /// CoRoutines are used to ensure it fades out changes position then fades in again
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameOverGood()
    {
        EventManager.instance.Fade(true);
        yield return new WaitForSeconds(6);
        VRplayer.transform.position = goodPosition.transform.position;
        EventManager.instance.Fade(false);
    }

    private IEnumerator GameOverBad()
    {
        EventManager.instance.Fade(true);
        yield return new WaitForSeconds(6);
        VRplayer.transform.position = badPosition.transform.position;
        EventManager.instance.Fade(false);

    }
    //Debug for testing
    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.O))
        //{
        //    EventManager.instance.WinGame();
        //}
    }
}