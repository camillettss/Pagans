using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Animator animator;
    [SerializeField] float speed = 5f;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] public LayerMask seaLayer;
    [SerializeField] public LayerMask farmingLayer;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Text questUI;
    [SerializeField] Light2D light;

    float moveLimiter = 0.7f;
    Rigidbody2D rb;
    Vector2 moveInput;
    float attackTime = 0.7f;
    float attackCounter;
    float arrowSpeed = 15f;
    bool toggleHotbar = false;

    [HideInInspector] public int maxHp = 30;

    [HideInInspector] public int gold=1;
    [HideInInspector] public int experience=0;
    [HideInInspector] public int hp = 10;

    [HideInInspector] public Quest quest;
    [HideInInspector] public static Player i;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public InventorySlot equipedItem = null;
    [HideInInspector] public QuestInventory QuestsContainer;

    public SceneDetails currentScene;
    public bool SnapToGridMovments = false;
    public int kents = 0;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        QuestsContainer = GetComponent<QuestInventory>();
        rb = GetComponent<Rigidbody2D>();
        attackCounter = attackTime;
        UpdateQuestUI();
        i = this;
    }

    public void HandleUpdate()
    {
        if (inventory.torch != null && inventory.torch.bright)
        {
            light.intensity = inventory.torch.brightness;
            light.pointLightInnerRadius = inventory.torch.radius;
            light.pointLightOuterRadius = inventory.torch.radius + 3f;
        }
        else
        {
            light.intensity = 0f;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if(moveInput != Vector2.zero)
        {
            animator.SetFloat("FacingHorizontal", moveInput.x);
            animator.SetFloat("FacingVertical", moveInput.y);
        }
        /*if(moveInput.x != 0 && moveInput.y != 0)
        {
            moveInput.x *= moveLimiter;
            moveInput.y *= moveLimiter;
        }*/

        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            moveInput.y = 0;
        else
            moveInput.x = 0;

        
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        rb.velocity = moveInput * speed;
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);

        if(animator.GetBool("Attacking")) // shoot on animation ends
        {
            rb.velocity = Vector2.zero;
            attackCounter -= Time.deltaTime;
            if(attackCounter <= 0)
            {
                animator.SetBool("Attacking", false);
                Shoot();
            }
        }

        // HANDLE INPUTS
        if (Input.GetKeyDown(KeyCode.E)) // Interact
        {
            // if possible interact with an npc, else use the equiped item.
            try
            {
                GetFrontalCollider().GetComponent<IEntity>().Interact(this);
            }
            catch(System.NullReferenceException)
            {
                if(equipedItem != null && equipedItem.item != null)
                {
                    equipedItem.item.Use(this);
                }
            }
            
        }

        if(Input.GetKeyDown(KeyCode.P)) // test feature key
        {
            
        }

        if (Input.GetKeyDown(KeyCode.F1) && !GameController.Instance.hotbar.isActiveAndEnabled)
        {
            // toggle hotbar visibility setting to the opposite state
            GameController.Instance.hotbar.gameObject.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.F1) && GameController.Instance.hotbar.isActiveAndEnabled)
        {
            // toggle hotbar visibility setting to the opposite state
            GameController.Instance.hotbar.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && inventory.torch != null && !currentScene.outdoor) // Toggle torch
            inventory.torch.Use(this);

        if (Input.GetKeyDown(KeyCode.L)) // Attack
            Attack();

        if (Input.GetKeyDown(KeyCode.Return)) // Menu
        {
            GameController.Instance.OpenState(GameState.Menu); // PERCHE MOSTRA SWORD NEL ENCHUI?????
        }

        if (Input.GetKeyDown(KeyCode.T)) // Bow
        {
            if(!animator.GetBool("Attacking"))
            {
                // this only start animation cuz this HandleUpdate() wait for animation to complete for shooting a bullet.
                attackCounter = attackTime;
                animator.SetBool("Attacking", true);
            }
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            GetFrontalCollider(farmingLayer).GetComponent<Plant>().Grow();
        }
    }

    private void FixedUpdate()
    {
        if (quest != null)
            if (quest.goal.isReached())
                quest.Complete();
    }

    public bool isInRange(IEntity entity, float radius=1.5f)
    {
        foreach(var item in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            if (item.GetComponent<IEntity>() == entity)
                return true;
        }
        return false;
    }

    public void UpdateQuestUI()
    {
        if(quest!=null)
        {
            questUI.text = quest.goal.goal;
        }
        else
        {
            questUI.text = "";
        }
    }

    public void AcceptQuest(Quest qst)
    {
        qst.isActive = true;
        quest = qst;
        UpdateQuestUI();
    }

    void Attack()
    {
        // start animation
        animator.SetTrigger("SwordAttack");
        animator.SetBool("Attacking", false);
        
        // use the equiped sword to hit the npc
        inventory.getEquiped("weapon").item.Use(this, GetFrontalCollider().GetComponent<IEntity>(), inventory.getEquiped("weapon").item.GetCloseDamage());
    }

    void Shoot()
    {
        // genera una freccia e spingila nella facing direction.
        var FirePos = new Vector3(transform.position.x+animator.GetFloat("FacingHorizontal"), transform.position.y+animator.GetFloat("FacingVertical"), 0);

        // set rotation
        var rotation = 0f;
        if (animator.GetFloat("FacingHorizontal") < 0)
            rotation = 90;
        else if (animator.GetFloat("FacingHorizontal") > 0)
            rotation = -90;
        else if (animator.GetFloat("FacingVertical") < 0)
        {
            rotation = 180;
            FirePos.y -= .5f;
        }

        // instantiate a bullet in the facing direction and grab its rigidbody
        var bullRb = Instantiate(bulletPrefab, FirePos, Quaternion.Euler(0, 0, rotation)).GetComponent<Rigidbody2D>();

        // bullet goes in the facing direction
        Vector2 vec = new Vector2(0, 0);
        if(animator.GetFloat("FacingHorizontal") != 0)
        {
            if (animator.GetFloat("FacingHorizontal") > 0)
                vec.x = arrowSpeed;
            else
                vec.x = -arrowSpeed;
        }
        else if(animator.GetFloat("FacingVertical") != 0)
        {
            if (animator.GetFloat("FacingVertical") > 0)
                vec.y = arrowSpeed;
            else
                vec.y = -arrowSpeed;
        }

        // apply the speed vector
        bullRb.velocity = vec;
    }

    public void GetLoot(ItemBase item)
    {
        inventory.GetSlots(item.category).Add(new InventorySlot(item));
    }

    public Collider2D GetFrontalCollider(LayerMask layer)
    {
        var facingDir = new Vector3(animator.GetFloat("FacingHorizontal"), animator.GetFloat("FacingVertical"));
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, layer);

        return collider;
    }

    // overload
    public Collider2D GetFrontalCollider()
    {
        return GetFrontalCollider(interactableLayer);
    }

    public void TakeDamage(int damageAmount)
    {
        hp -= damageAmount;
    }
}
