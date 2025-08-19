using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] GameObject[] itemPrefabs; // �����ɵĵ���Ԥ����
    [SerializeField] float spawnInterval = 5f; // ���ɼ�����룩

    [Header("Spawn Area")]
    [SerializeField] Vector2 spawnMin = new Vector2(-10f, -5f); // ����������Сֵ
    [SerializeField] Vector2 spawnMax = new Vector2(10f, 5f);   // �����������ֵ

    [Header("Check Settings")]
    [SerializeField] float checkRadius = 0.5f; // ���뾶����������������ص�
    [SerializeField] LayerMask groundLayer;    // ������ϰ����

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
        // ���ѡ��һ������Ԥ����
        if (itemPrefabs.Length == 0) return;
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        // ��һ���Ϸ������λ��
        Vector2 spawnPos;
        int maxAttempts = 20; // ��ೢ�� 20 ����λ��
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
            Debug.LogWarning("δ�ҵ����ʵ�����λ�ã�������������");
            return;
        }

        // ���ɵ���
        GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

    }
}
