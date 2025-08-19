using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] GameObject[] itemPrefabs; // 可生成的道具预制体
    [SerializeField] float spawnInterval = 5f; // 生成间隔（秒）

    [Header("Spawn Area")]
    [SerializeField] Vector2 spawnMin = new Vector2(-10f, -5f); // 生成区域最小值
    [SerializeField] Vector2 spawnMax = new Vector2(10f, 5f);   // 生成区域最大值

    [Header("Check Settings")]
    [SerializeField] float checkRadius = 0.5f; // 检测半径，避免和已有物体重叠
    [SerializeField] LayerMask groundLayer;    // 地面或障碍物层

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnItem();
            timer = 0f;
        }
    }

    void SpawnItem()
    {
        // 随机选择一个道具预制体
        if (itemPrefabs.Length == 0) return;
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        // 找一个合法的随机位置
        Vector2 spawnPos;
        int maxAttempts = 20; // 最多尝试 20 次找位置
        int attempts = 0;

        do
        {
            float x = Random.Range(spawnMin.x, spawnMax.x);
            float y = Random.Range(spawnMin.y, spawnMax.y);
            spawnPos = new Vector2(x, y);
            attempts++;
        }
        while (Physics2D.OverlapCircle(spawnPos, checkRadius, groundLayer) != null && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("未找到合适的生成位置，跳过本次生成");
            return;
        }

        // 生成道具
        GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

    }
}
