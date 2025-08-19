using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Laser }   // ��������
    [SerializeField] ItemType itemType = ItemType.Laser;
    [SerializeField] float lifeTime = 10f; // ���ߴ���ʱ��

    private void Start()
    {
        Destroy(gameObject, lifeTime); // һ��ʱ����Զ���ʧ
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Shoot shoot = collision.GetComponent<Shoot>();
        if (shoot != null)
        {
            switch(itemType)
            {
                case ItemType.Laser:
                    {
                        shoot.EnableLaserMode();
                        break;
                    }
                default:
                    break;
            }
            
            Destroy(gameObject); // ��ʰȡ�����ٵ���
        }
    }
}
