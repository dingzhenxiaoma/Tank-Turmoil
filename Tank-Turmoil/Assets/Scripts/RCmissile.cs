using UnityEngine;

public class RCmissile : Bullet
{
    private Shoot owner;
    private float speed;
    private float lifeTime;
    private Shoot.PlayerType playerType;

    private float timer = 0f;
    private Rigidbody2D rb;

    public void Init(Shoot owner, float speed, float lifeTime, Shoot.PlayerType playerType)
    {
        this.owner = owner;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.playerType = playerType;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Dead();
            return;
        }

        // 控制旋转
        float turn = 0f;
        if (playerType == Shoot.PlayerType.Player1)
        {
            if (Input.GetKey(KeyCode.A)) turn = 1f;
            else if (Input.GetKey(KeyCode.D)) turn = -1f;
        }
        else if (playerType == Shoot.PlayerType.Player2)
        {
            if (Input.GetKey(KeyCode.J)) turn = 1f;
            else if (Input.GetKey(KeyCode.L)) turn = -1f;
        }

        // 修改方向
        transform.Rotate(0, 0, turn * 120f * Time.deltaTime); // 每秒120度转向
        rb.velocity = transform.up * speed;
    }

    public override void Dead()
    {
        base.Dead();
        Player player= owner.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.EnableMove();
        }

        owner.ClearRCmissileReference();
        Destroy(gameObject);
    }
}
