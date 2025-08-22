using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI tmpTextPlayer1; // ���� TMP ���Player1
    public TextMeshProUGUI tmpTextPlayer2; // ���� TMP ���Player2

    void Start()
    {
        // ��ʼ���ı�����
        tmpTextPlayer1.text = "0";
        tmpTextPlayer2.text = "0";
    }

    void Update()
    {
        int score1 = ScoreManager.Instance.getScore(1);
        int score2 = ScoreManager.Instance.getScore(2);
        // ��̬�����ı�
        tmpTextPlayer1.text = score1.ToString();
        tmpTextPlayer2.text = score2.ToString();
    }
}
