using System;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] int PlayerNum = 2;//��Ҹ���

    private const int MaxPlayerNum = 3;//�����Ҹ��� 
    public static ScoreManager Instance { get; private set; }

    private bool[] isDeadArray = new bool[MaxPlayerNum+1]; // ��� 3 �����
    private int[] scoreArray = new int[MaxPlayerNum + 1]; // ��� 3 �����

    private void Start()
    {
        reload();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �г���������
        }
        else
        {
            Destroy(gameObject); // �����ظ�ʵ��
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
            if (!isDeadArray[i]) // �ж� false
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
