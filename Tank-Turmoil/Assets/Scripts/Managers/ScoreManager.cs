using System;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] int PlayerNum = 2;//玩家个数

    private const int MaxPlayerNum = 3;//最大玩家个数 
    public static ScoreManager Instance { get; private set; }

    private bool[] isDeadArray = new bool[MaxPlayerNum+1]; // 最大 3 个玩家
    private int[] scoreArray = new int[MaxPlayerNum + 1]; // 最大 3 个玩家

    private void Start()
    {
        reload();
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

    public void reload()
    {
        System.Array.Fill(isDeadArray, false);
        System.Array.Fill(scoreArray, 0);
    }

    public void chgState(int id)
    {
        isDeadArray[id] = true;
        int winner = checkIfWin();

        if (winner != -1)
        {
            AddScore(winner);
            EventManager.Instance.reStart();
            System.Array.Fill(isDeadArray, false);
        }
        //show();
    }

    private int checkIfWin()
    {
        int index = -1;
        int count = 0;

        for (int i = 1; i <= PlayerNum; i++)
        {
            if (!isDeadArray[i]) // 判断 false
            {
                count++;
                index = i;
            }
        }

        return count == 1 ? index : -1;
    }

    private void AddScore(int id)
    {
        scoreArray[id]++;
    }

    public int getScore(int id)
    {
        Debug.Log(scoreArray[id]);
        return scoreArray[id];

    }

    private void show()
    {
        Debug.Log("now: ");
        for (int i = 1; i <= PlayerNum; i++)
        {
            Debug.Log(i +": "+ scoreArray[i]);
        }
    }
}
