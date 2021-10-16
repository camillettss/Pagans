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
    [SerializeField] Image ActiveQuestBG;
    [SerializeField] public Light2D torchLight;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange;

    float moveLimiter = 0.7f;
    Rigidbody2D rb;
    public Vector2 moveInput;
    float attackTime = 0.7f;
    float attackCounter;
    float arrowSpeed = 15f;
    bool toggleHotbar = false;

    [HideInInspector] public int maxHp = 30;

    [HideInInspector] public int gold=1;
    [HideInInspector] public int experience=0;
    [HideInInspector] public int hp = 10;

    [HideInInspector] public Quest quest = null;
    [HideInInspector] public static Player i;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public InventorySlot equipedItem = null;
    [HideInInspector] public QuestInventory QuestsContainer;
    [HideInInspector] public Altar activeAltar = null;
    [HideInInspector] public Plant activePlant = null;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canShowMinimap = true;

    public SceneDetails currentScene;
    public bool SnapToGridMovments = false;
    public int kents = 0;

    #region saving stuffs
    public bool isFirstLaunch = false;
    public int storyProgressValue = 0;
    #endregion

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

        if(canMove)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            animator.SetFloat("Speed", moveInput.sqrMagnitude);

            rb.velocity = moveInput * speed;
            rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
        }

        attackPoint.position = new Vector3(transform.position.x+animator.GetFloat("FacingHorizontal"), transform.position.y+animator.GetFloat("FacingVertical"), transform.position.z);

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

        // Update quest goals
        if(quest != null && (quest.goal != null || quest.goal.Count > 0))
        {
            if(quest.goal[0].isReached())
            {
                if (quest.goal.Count == 1)
                    quest.Complete();
                else
                    quest.goal.RemoveAt(0);

                UpdateQuestUI();
            }
        }

        // HANDLE INPUTS

        // BINDS: E: use, Z: interact, X: shield

        // minimap show
        if (Input.GetKeyDown(KeyCode.Q) && canShowMinimap && !GameController.Instance.MinimapCanvas.activeSelf)
            GameController.Instance.MinimapCanvas.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Q))
            GameController.Instance.MinimapCanvas.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Z)) // Interact
        {
            // use altar, else interact.
            if (activeAltar != null)
                StartCoroutine(activeAltar.Use());
            else
                interact();
        }

        if (Input.GetKeyDown(KeyCode.Space) && inventory.torch != null && !currentScene.outdoor) // Toggle torch
            inventory.torch.Use(this);

        if (Input.GetKeyDown(KeyCode.Return)) // Menu
            GameController.Instance.OpenState(GameState.Menu);

        if (Input.GetKeyDown(KeyCode.R)) // Attack
        {
            if(inventory.equipedWeapon != -1)
            {
                if (inventory.Weapons[inventory.equipedWeapon].item.longDamage == 0) // arma da vicino (spada)
                    useWeapon();
                else
                    useBow();
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) // Use
            useItem();

        if (Input.GetKeyDown(KeyCode.G)) // Shield
            useShield();

        if (Input.GetKeyDown(KeyCode.T))
        {
            var temp = inventory.equipedWeapon;
            inventory.equipedWeapon = inventory.secondaryWeapon;
            inventory.secondaryWeapon = temp;
            GameController.Instance.hotbar.UpdateItems();
        }
    }

    void useWeapon()
    {
        print(inventory.equipedWeapon);
        if (inventory.equipedWeapon != -1)
            inventory.Weapons[inventory.equipedWeapon].item.Use(this); // trova l'arma e usala
    }

    void useBow()
    {
        
        if (!animator.GetBool("Attacking"))
        {
            // this only start animation cuz this HandleUpdate() wait for animation to complete for shooting a bullet.
            attackCounter = attackTime;
            animator.SetBool("Attacking", true);
        }
    }

    void useShield()
    {
        print("Using the shield");
    }

    void useItem()
    {
        print("equiped:" + inventory.equipedTool);
        if (inventory.equipedTool != -1)
            inventory.Tools[inventory.equipedTool].item.Use(this);
    }

    void interact()
    {
        var front = GetFrontalCollider();
        if (front.TryGetComponent(out NPCController npc))
        {
            if (npc.type == NPCType.Enemy)
                throw new System.NullReferenceException();

            npc.Interact(this);
        }
        else if (front.TryGetComponent(out IEntity entity))
        {
            entity.Interact(this);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void Save()
    {
        SaveSystem.SavePlayer(this);
    }

    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if (data == null)
            data = PlayerData.emtpy;

        Load(data);
    }

    public void Load(PlayerData data)
    {
        hp = data.health;
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        print("[*] is first launch: " + data.firstLaunch);
        isFirstLaunch = data.firstLaunch;
        GameController.Instance.storyController.FirstTime_inventory = data.firstinventory;
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
        if(quest!=null && quest.goal != null)
        {
            // enable quests on hud cuz instead of removing text now i'll turn off the container gameobj.
            ActiveQuestBG.gameObject.SetActive(true);

            // now set text
            questUI.text = quest.goal[0].goal;

            if(quest.goal[0].goal == "")
                ActiveQuestBG.gameObject.SetActive(false);
        }
        else
        {
            // only disable background image
            ActiveQuestBG.gameObject.SetActive(false);
        }
    }

    public void AcceptQuest(Quest qst)
    {
        if (qst.title == "L'origine del mondo")
            StartCoroutine(GameController.Instance.storyController.AsbjornQuestAccepted());
        qst.isActive = true;
        quest = qst;
        UpdateQuestUI();
    }

    public void Attack(Weapon weapon)
    {
        // start animation
        animator.SetTrigger("SwordAttack");
        animator.SetBool("Attacking", false);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, interactableLayer);

        foreach (Collider2D enemy in hitEnemies)
            enemy.GetComponent<IEntity>().takeDamage(inventory.Weapons[inventory.equipedWeapon].item.GetCloseDamage());
        
        // use the equiped sword to hit the npc
        //inventory.getEquiped("weapon").item.Use(this, GetFrontalCollider().GetComponent<IEntity>(), inventory.getEquiped("weapon").item.GetCloseDamage());
    }

    public void Shoot()
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
