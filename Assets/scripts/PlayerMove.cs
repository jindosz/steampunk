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

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && rollTime > 0.7f)
        {
            anim.SetBool("isRolling", true);
            rollTime = 0f;
        }

        LimitVelocity();

        if (Input.GetButtonDown("Jump"))
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

        rigid.AddForce(Vector2.right * h * walkForce, ForceMode2D.Impulse);

        jumpTime += Time.deltaTime;
        rollTime += Time.deltaTime;

        if (jumpTime <= 0.2f)
            rigid.AddForce(Vector2.up * jump, ForceMode2D.Force);

        if (rollTime <= 0.6f)
        {
            rigid.AddForce(Vector2.right * rollingPower * Mathf.Sign(h), ForceMode2D.Force);
        }
        else
            anim.SetBool("isRolling", false);
    }

    void LimitVelocity()
    {
        bool run = Input.GetKey(KeyCode.LeftControl);

        if (rollTime > 0.6f)
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
}
