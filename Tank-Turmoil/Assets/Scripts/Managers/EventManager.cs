using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("PlayerPrefab")]
    [SerializeField] GameObject Player1;
    [SerializeField] GameObject Player2;

    [Header("Position")]
    [SerializeField] Transform posPlayer1;
    [SerializeField] Transform posPlayer2;

    GameObject player1Obj = null;
    GameObject player2Obj = null;

    public static EventManager Instance { get; private set; }

    private void Start()
    {
        reSetPos();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切场景不销毁
        }
        else
        {
            Destroy(gameObject); // 避免重复实例
        }
    }

    public void reStart()
    {
        StartCoroutine(SleepReStart());
    }

    IEnumerator SleepReStart()
    {
        yield return new WaitForSeconds(1f);
        if (player1Obj != null)
            Destroy(player1Obj);
        if (player2Obj != null)
            Destroy(player2Obj); 
        reSetPos();
    }

    private void reSetPos()
    {
        player1Obj = Instantiate(Player1, posPlayer1.position,posPlayer1.rotation);
        player2Obj = Instantiate(Player2, posPlayer2.position, posPlayer2.rotation);
    }
}
