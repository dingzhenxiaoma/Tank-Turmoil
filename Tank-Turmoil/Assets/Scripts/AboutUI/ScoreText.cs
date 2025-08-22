using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI tmpTextPlayer1; // 引用 TMP 组件Player1
    public TextMeshProUGUI tmpTextPlayer2; // 引用 TMP 组件Player2

    void Start()
    {
        // 初始化文本内容
        tmpTextPlayer1.text = "0";
        tmpTextPlayer2.text = "0";
    }

    void Update()
    {
        int score1 = ScoreManager.Instance.getScore(1);
        int score2 = ScoreManager.Instance.getScore(2);
        // 动态更新文本
        tmpTextPlayer1.text = score1.ToString();
        tmpTextPlayer2.text = score2.ToString();
    }
}
