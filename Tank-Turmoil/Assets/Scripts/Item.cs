using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { Laser,Frag,Deathray }   // 道具类型
    [SerializeField] ItemType itemType;
    [SerializeField] float lifeTime = 10f; // 道具存在时间

    private void Start()
    {
        Destroy(gameObject, lifeTime); // 一段时间后自动消失
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
                case ItemType.Frag:
                    {
                        shoot.EnableFragMode();
                        break;
                    }
                case ItemType.Deathray:
                    {
                        shoot.EnableDeathrayMode();
                        break;
                    }
                default:
                    break;
            }
            
            Destroy(gameObject); // 被拾取后销毁道具
        }
    }
}
