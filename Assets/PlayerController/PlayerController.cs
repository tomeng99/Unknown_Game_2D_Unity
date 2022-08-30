using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed;
    public float jumpForce;

    public bool onGround;

    private Rigidbody2D rb;
    private Animator anim;

    private float horizontal;

    [HideInInspector]
    public Vector2Int worldPosition;
    public TerrainGeneration terrainGenerator;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        // Temporary
        if (!IsOwner)
        {
            PlayerController pc = GetComponent<PlayerController>();
            pc.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
    private void FixedUpdate()
    {
        // Do stuff
        horizontal = Input.GetAxisRaw("Horizontal");
        float jump = Input.GetAxisRaw("Jump");

        if (IsOwner)
        {
            if (horizontal > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);

            }
            else if (horizontal < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            if (jump > 0.1f)
            {
                if (onGround)
                    movement.y = jumpForce;

            }

            rb.velocity = movement;

        }

        float fire = Input.GetAxisRaw("Fire1");

        if (fire > 0)
        {
            terrainGenerator.RemoveTile(worldPosition.x, worldPosition.y);
        }
    }

    private void Update()
    {
        worldPosition.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 0.5f);
        worldPosition.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y + 0.5f);

        anim.SetFloat("horizontal", horizontal);
    }

}
