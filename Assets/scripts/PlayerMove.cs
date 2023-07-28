using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxWalkSpeed;
    public float maxRunSpeed;
    public float jump;
    public float rollingPower;
    public float walkForce;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    private float jumpTime = 10f;
    private float rollTime = 10f;
    bool run = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        CheckGround();

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            run = !run;
        }

        if (Input.GetKeyDown(KeyCode.Q) && rollTime > 0.7f && CheckGround())
        {
            anim.SetTrigger("Roll");
            rollTime = 0f;
        }

        LimitVelocity();

        if (Input.GetButtonDown("Jump") && CheckGround())
        {
            jumpTime = 0f;
            anim.SetBool("isJumping", true);
        }

        if (Mathf.Abs(rigid.velocity.y) < 0.1 && jumpTime > 0.3f)
        {
            anim.SetBool("isJumping", false);
        }

        if (Input.GetButtonUp("Horizontal")) // 멈추기
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

        if (rigid.velocity.x < -0.1) // 캐릭터 방향 전환
            spriteRenderer.flipX = true;
        else if (rigid.velocity.x > 0.1)
            spriteRenderer.flipX = false;

        if (Mathf.Abs(rigid.velocity.x) <= 0.2) // 대기 <-> 걷기 <-> 뛰기 애니메이션 전환
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
        }
        else if (Mathf.Abs(rigid.velocity.x) > maxWalkSpeed)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }
        else if (Mathf.Abs(rigid.velocity.x) > 0.2 && Mathf.Abs(rigid.velocity.x) <= maxWalkSpeed)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }

        anim.SetFloat("velocityY", rigid.velocity.y);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if (rollTime > 0.6f)
            rigid.AddForce(Vector2.right * h * walkForce, ForceMode2D.Impulse);

        jumpTime += Time.deltaTime;
        rollTime += Time.deltaTime;

        if (jumpTime <= 0.2f)
            rigid.AddForce(Vector2.up * jump, ForceMode2D.Force);

        if (rollTime <= 0.6f)
        {
            rigid.AddForce(
                Vector2.right * rollingPower * (spriteRenderer.flipX ? -1 : 1),
                ForceMode2D.Force
            );
        }
    }

    void LimitVelocity()
    {
        if (rollTime > 0.55f)
        {
            if (run)
            {
                if (Mathf.Abs(rigid.velocity.x) > maxRunSpeed)
                {
                    rigid.velocity = new Vector2(
                        Mathf.Sign(rigid.velocity.x) * maxRunSpeed,
                        rigid.velocity.y
                    );
                }
            }
            else
            {
                if (Mathf.Abs(rigid.velocity.x) > maxWalkSpeed)
                {
                    rigid.velocity = new Vector2(
                        Mathf.Sign(rigid.velocity.x) * maxWalkSpeed,
                        rigid.velocity.y
                    );
                }
            }
        }
    }

    public bool CheckGround()
    {
        float distanceToTheGround = GetComponent<Collider2D>().bounds.extents.y;
        return BoxCastDrawer.BoxCastAndDraw(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(0.45f, 0.8f), //Size
            0f,
            Vector2.down,
            distanceToTheGround + 0.05f,
            ~LayerMask.GetMask("Player")
        );
    }
}
