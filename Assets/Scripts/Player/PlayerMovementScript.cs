using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovementScript : MonoBehaviour
{
    //Scripts
    PlayerStatsScript playerStats;
    public DashChargeBarScript dashChargeBar;

    //Movement - left and right
    [SerializeField] private float facingValue = 1;
    public GameObject rotationPoint;
    public bool canMove = true;
    private bool isFacingLeft;
    public float horizontal;

    //Movement - jump
    public double jumpingTime = 0.45;
    public bool isJumping;

    //Platformy
    public CapsuleCollider2D playerCollider;
    public GameObject currentOneWayPlatform;

    //Movement - dash
    [SerializeField] public bool isDashing;
    [SerializeField] private bool dashTrigger;
    [SerializeField] private bool canDash = true;
    public TrailRenderer dashTrail;


    //Respawn
    [SerializeField] private float deadzone = -10;
    public GameObject respawnZone;

    public Rigidbody2D playerRigidBody;
    public LayerMask groundLayer;
    public GameObject jumpControl;
    public Animator playerAnimator;

    private void Start()
    {
        playerStats = GetComponent<PlayerStatsScript>();
        dashChargeBar = GameObject.FindGameObjectWithTag("PlayerUI").GetComponentInChildren<DashChargeBarScript>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<CapsuleCollider2D>();

        if(playerStats != null)
        {
            dashTrail.emitting = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(horizontal);
        //Movement
        if (playerAnimator != null && !PlayerUIScript.GameIsPaused)
        {
            if (isDashing)
            {
                dashChargeBar.UpdateChargeBarImage(2, 2);
                return;
            }

            playerAnimator.SetFloat("playerMovementSpeed", Mathf.Abs(horizontal));  //bere hodnotu z horizontal, aby se mohl pou��t playerMovementSpeed v animatoru
            playerAnimator.SetBool("isJumping", isJumping);
            playerAnimator.SetBool("PlayerOnGround", Grounded());

            flipCharacter();

            if (canMove)
            {
                    horizontal = Input.GetAxis("Horizontal");   //Chozen� doprava a doleva
                    playerRigidBody.velocity = new Vector2(horizontal * playerStats.playerMovementSpeed, playerRigidBody.velocity.y);

                }

                if (Input.GetButtonDown("Jump") & Grounded())   //Kontrola jestli je na zemi, tak m��e sko�it
                {
                    playerAnimator.Play("Jumping", 0, 0f);
                    playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, playerStats.playerJumpPower);
                    playerRigidBody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                playerRigidBody.gravityScale = 2f;
                if (currentOneWayPlatform != null) {
                    StartCoroutine(DisableCollision());
                }
            }

            else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                playerRigidBody.gravityScale = 1f;
            }

            if (playerRigidBody.velocity.y > 0 && playerRigidBody.velocity.y <= 7.5 && !Grounded())     // kdy� hr�� ud�l� skok (nahoru), tak se zahraje animace
            {
                isJumping = true;
            }

            else if (playerRigidBody.velocity.y < 0 || playerRigidBody.velocity.y > 7.5 && Grounded())
            {
                isJumping = false;
            }

            dashFunction();
        }
        //Respawn
        if (playerRigidBody.position.y < deadzone)
        {
            playerRigidBody.position = respawnZone.transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
    }

    public bool Grounded()
    {    //vrac� hodnotu, kdy� se kruh o polom�ru 0.2f dotkne groundLayer
        return Physics2D.OverlapCircle(jumpControl.transform.position, 0.2f, groundLayer);
    }

    public void flipCharacter()
    {   //Funkce pro oto�en� charaktera pokud chod� doprava nebo doleva
        if (!isFacingLeft && horizontal < 0f || isFacingLeft && horizontal > 0f) {
            isFacingLeft = !isFacingLeft;
            transform.Rotate(0f, 180f, 0f);
            facingValue = -facingValue;
        }
    }

    public void dashFunction()
    {
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash == true && playerStats.playerDashesAvailable != 0)
            {
            StartCoroutine(Dash());
            }

            if (Grounded() && playerStats.playerDashesAvailable >= 0 && playerStats.playerDashesAvailable != playerStats.playerNumberOfDashes && isDashing == false && dashTrigger == false)   //Zkontroluje jestli je na zemi (Grounded()), zda m� po�et dostupn�ch dash� (dashesAvailable) men�� ne� po�et dash� (numberOfDashes), jestli zrovna dashuje (isDashing) a dashTrigger se d� false,
                                                                                                                                                                                               //kdy� se po�k� 2 vte�iny (dashingCooldown) v dashCooldown funkci, pak se d� true a zaru��, �e se ta funkce vykon� pouze jednou
            {
                dashTrigger = true;
                StartCoroutine(dashCooldown());
            }

            IEnumerator Dash()
            {
                isDashing = true;
                PlayerStatsScript.isPlayerInvincible = true;
                dashTrail.emitting = true;
                canDash = false;
                playerStats.playerDashesAvailable = playerStats.playerDashesAvailable + (playerStats.playerDashesAvailable - (playerStats.playerDashesAvailable + 1));
                /*float originalGravity = playerRigidBody.gravityScale;
                playerRigidBody.gravityScale = 0;*/
                playerRigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; //Zmraz� pozici Y a rotaci Z
                playerRigidBody.velocity = new Vector2(playerStats.playerDashPower * facingValue, 0);
                playerAnimator.Play("Dash", 0, 0f);
                yield return new WaitForSeconds(playerStats.playerDashingTime);
                playerRigidBody.constraints &= ~RigidbodyConstraints2D.FreezePositionY; //Rozmraz� pozici Y
                /*if (originalGravity == 0)
                {
                    playerRigidBody.gravityScale = originalGravity;
                }
                else if (originalGravity > 0) {
                    originalGravity = 1;
                    playerRigidBody.gravityScale = originalGravity;
                }*/
                dashTrail.emitting = false;
                PlayerStatsScript.isPlayerInvincible = false;
                isDashing = false;
                if (playerStats.playerDashesAvailable > 0)
                {
                    canDash = true;
                }
                else if (playerStats.playerDashesAvailable == 0)
                {
                    canDash = false;
                }
            }

            IEnumerator dashCooldown()
            {
                float currentdashesAvailable = playerStats.playerDashesAvailable;
                {
                    for (double timer = playerStats.playerDashingCooldown + 1; timer >= 0; timer -= Time.deltaTime)     //Ka�d� 2 vte�iny se obnov� dash
                    {
                        dashChargeBar.UpdateChargeBarImage(timer-1, 2);
                        if (playerStats.playerDashesAvailable != currentdashesAvailable)  //Pokud dashne v pr�b�hu �asu, co se mu obnovuje dash, tak se za�ne �as po��tat od znova
                        {
                        timer = 2;
                            dashTrigger = false;
                            yield break;
                        }
                        if (timer < 1)
                        {    //Pokud po�k� dokud prob�hne �as, tak se mu navr�t� v�echny dashe
                            playerStats.playerDashesAvailable = playerStats.playerNumberOfDashes;
                            dashTrigger = false;
                            canDash = true;
                            yield break;
                        }
                        yield return null;
                    }
                }
            }
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isDashing", isDashing);
            }
        }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform")) {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision() { 
        TilemapCollider2D platformCollider = currentOneWayPlatform.GetComponent<TilemapCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
