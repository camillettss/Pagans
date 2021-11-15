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
    [SerializeField] public Transform attackPoint;
    [SerializeField] public float attackRange;
    public float plantRange;

    float moveLimiter = 0.7f;
    Rigidbody2D rb;
    public Vector2 moveInput;
    float attackTime = 0.7f;
    float attackCounter;
    float arrowSpeed = 15f;
    bool toggleHotbar = false;
    float walkingSpeedDefault = 5f;

    [HideInInspector] public int maxHp = 30;

    [HideInInspector] public int gold=1;
    [HideInInspector] public int experience=9;
    [HideInInspector] public int hp = 10;

    [HideInInspector] public Quest quest = null;
    [HideInInspector] public static Player i;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public InventorySlot equipedItem = null;
    [HideInInspector] public QuestInventory QuestsContainer;
    [HideInInspector] public Altar activeAltar = null;
    //[HideInInspector] public Plant activePlant = null;
    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canShowMinimap = true;
    public Key keyToUse = null;
    [HideInInspector] public Door closeDoor = null;
    [HideInInspector] public Horse activeHorse;

    [HideInInspector] public int defense; // quando attaccano il danno subito è danno-defense 

    public SceneDetails currentScene;
    public bool SnapToGridMovments = false;
    public int kents = 0;

    float ridingSpeed = 10f;
    float runningSpeed = 8f;
    float holdingShieldSpeed = 2.5f;

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
        experience = 9;
    }

    private void Start()
    {
        // questo viene dopo l'awake
        //AstarPath.active.Scan(); // AI things are disabled now
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

            rb.velocity = moveInput * getSpeed();
            rb.MovePosition(rb.position + moveInput * getSpeed() * Time.fixedDeltaTime);
        }

        attackPoint.position = new Vector3(transform.position.x+animator.GetFloat("FacingHorizontal"), transform.position.y+animator.GetFloat("FacingVertical"), transform.position.z);

        if (inventory.equipedShield != -1)
            animator.SetBool("hasShield", true);
        else
            animator.SetBool("hasShield", false);

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
        if(quest != null)
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

        // BINDS: E: use, R: use weapon, Z: interact, F: use xtra, X: shield, Q: minimap, LShift: run

        // minimap show
        if (Input.GetKeyDown(KeyCode.Q) && canShowMinimap && !GameController.Instance.MinimapCanvas.activeSelf)
            GameController.Instance.MinimapCanvas.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Q))
            GameController.Instance.MinimapCanvas.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Z)) // Interact
        {
            // passes through Event Handler.
            GameController.Instance.EvH.Interact();
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            if (activeHorse != null)
                Dismount();
        }

        /*if (Input.GetKeyDown(KeyCode.Space) && inventory.torch != null && !currentScene.outdoor) // Toggle torch
            inventory.torch.Use(this);*/

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
        {
            if(GameController.Instance.keyUI.isActiveAndEnabled)
            {
                keyToUse.Use(this);
                inventory.Remove(keyToUse);
                closeDoor.Open();
            } else
            {
                useItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) // Shield
        {
            if(inventory.equipedShield != -1)
            {
                defense = inventory.Shields[inventory.equipedShield].item.GetDefense();
                useShield();
            }
            
        }
        if (Input.GetKeyUp(KeyCode.X)) // off
        {
            if(inventory.equipedShield != -1)
            {
                defense = 0;
                animator.SetBool("holdingShield", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            var temp = inventory.equipedWeapon;
            inventory.equipedWeapon = inventory.secondaryWeapon;
            inventory.secondaryWeapon = temp;
            GameController.Instance.hotbar.UpdateItems();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            if (inventory.extraSlot != null && inventory.extraSlot.item != null)
                inventory.extraSlot.item.Use(this);
        }
    }

    public void Ride(Horse horse)
    {
        animator.SetBool("isRiding", true);
        horse.gameObject.SetActive(false);
        activeHorse = horse;
    }

    public void Dismount()
    {
        animator.SetBool("isRiding", false);
        activeHorse.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        activeHorse.gameObject.SetActive(true);
        activeHorse = null;
    }

    void useWeapon()
    {
        rb.velocity = Vector2.zero;
        print(inventory.equipedWeapon);
        if (inventory.equipedWeapon != -1)
            inventory.Weapons[inventory.equipedWeapon].item.Use(this); // trova l'arma e usala
    }

    void useBow()
    {
        rb.velocity = Vector2.zero;
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
        animator.SetBool("holdingShield", true);
        animator.SetTrigger("useShield");
    }

    void useItem()
    {
        print("equiped:" + inventory.equipedTool);
        if (inventory.equipedTool != -1)
            inventory.Tools[inventory.equipedTool].item.Use(this);
    }

    public void Cut()
    {
        var tree = GetFrontalCollider(farmingLayer);

        if(tree != null && tree.TryGetComponent(out Tree tempTree))
        {
            Debug.Log("cutting");
            tempTree.Cut();
        }
    }

    float getSpeed()
    {
        if (animator.GetBool("isRiding"))
            return ridingSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            return runningSpeed;
        else if (animator.GetBool("holdingShield"))
            return holdingShieldSpeed;
        else
            return walkingSpeedDefault;
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

        if (data == null || data.firstLaunch == true)
            data = PlayerData.emtpy;

        Load(data);
    }

    public void Load(PlayerData data)
    {
        hp = data.health;
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        print("[*] is first launch: " + data.firstLaunch);
        GameController.Instance.storyController.firstLaunch = data.firstLaunch; ;
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
        //if (qst.title == "L'origine del mondo")
        //    StartCoroutine(GameController.Instance.storyController.AsbjornQuestAccepted());
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
        return GetFrontalCollider(interactableLayer | farmingLayer);
    }

    public void TakeDamage(int damageAmount)
    {
        hp -= damageAmount;
    }
}

/*namespace Pagans
{
    public static class PlayerThings
    {
        public static Player GetPlayerInstance()
        {
            return Player.i;
        }
    }
}*/