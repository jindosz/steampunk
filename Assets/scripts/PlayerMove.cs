using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxWalkSpeed;
    public float maxRunSpeed;
    public float jump;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
            rigid.AddForce(Vector2.up * jump, ForceMode2D.Impulse);

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
        else if (Mathf.Abs(rigid.velocity.x) >= maxWalkSpeed)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }
        else if (Mathf.Abs(rigid.velocity.x) > 0.2 && Mathf.Abs(rigid.velocity.x) < maxWalkSpeed)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        bool run = false;

        if (Input.GetKey(KeyCode.LeftControl) == true) //달리기 감지
            run = true;
        else if (Input.GetKey(KeyCode.LeftControl) == false)
            run = false;

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (run == false && rigid.velocity.x > maxWalkSpeed)
            rigid.velocity = new Vector2(maxWalkSpeed, rigid.velocity.y);
        else if (run == false && rigid.velocity.x < maxWalkSpeed * (-1))
            rigid.velocity = new Vector2(maxWalkSpeed * (-1), rigid.velocity.y);

        if (run == true && rigid.velocity.x > maxRunSpeed)
            rigid.velocity = new Vector2(maxRunSpeed, rigid.velocity.y);
        else if (run == true && rigid.velocity.x < maxRunSpeed * (-1))
            rigid.velocity = new Vector2(maxRunSpeed * (-1), rigid.velocity.y);
    }
}
