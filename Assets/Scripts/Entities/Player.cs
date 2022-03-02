using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player : MonoBehaviour
{
    public Animator animator;
    [SerializeField] float speed = 5f;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] public LayerMask seaLayer;
    [SerializeField] public LayerMask farmingLayer;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Text questUI;
    [SerializeField] Text questUIfraction;
    [SerializeField] Image ActiveQuestBG;
    [SerializeField] public Light2D torchLight;
    [SerializeField] public Transform attackPoint;
    [SerializeField] Transform transportPoint;
    [SerializeField] public float attackRange;
    public float plantRange;

    // input sys
    public InputActionMap UIInputMap;
    public PlayerInput playerInput;

    public List<StoryBook> Recipes;
    public List<StoryBook> GodsBooks;
    public List<StoryBook> ElvesBooks;
    public List<StoryBook> MonstersBooks;
    public List<StoryBook> StoriesBooks;

    float moveLimiter = 0.7f;
    Rigidbody2D rb;
    public Vector2 moveInput;
    float attackTime = 0.7f;
    float attackCounter;
    float arrowSpeed = 15f;
    public Transform Head;

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
    //[HideInInspector] public Plant activePlant = null;

    public bool canMove = true;
    public bool canJump = false;
    public bool isFishing = false;
    public bool canAttack = true;
    public bool canUseTool = true;

    [HideInInspector] public bool canShowMinimap = true;
    public Key keyToUse = null;
    [HideInInspector] public Door closeDoor = null;
    [HideInInspector] public Horse activeHorse;

    [HideInInspector] public int defense; // quando attaccano il danno subito è danno-defense TODO

    public SceneDetails currentScene;
    public SceneDetails prevScene;

    public bool SnapToGridMovments = false;
    public int kents = 0;
    public CityDetails triggeredCity=null;

    public Port activePort;
    public Agrimap activeAgrimap;
    public AgribleTile activePlant;

    public Animal transportingAnimal = null;
    public LiftableItem liftingItem = null;

    public InteractableObject activeBench;
    public Boat activeBoat = null;
    public Boat targetBoat = null;

    public Camera cam;

    public bool drawSelected_Agrimap = true;

    float ridingSpeed = 8.2f;
    float runningSpeed = 5f;
    float walkingSpeedDefault = 3f;
    float holdingShieldSpeed = 2.5f;

    public bool enableDiagonalMovements = false;

    public bool teleporting = false;

    public ParticleSystem confettis;

    #region saving stuffs
    public bool isFirstLaunch = false;
    public int storyProgressValue = 0;
    #endregion

    private void Awake()
    {
        i = this;

        inventory = GetComponent<Inventory>();
        QuestsContainer = GetComponent<QuestInventory>();
        rb = GetComponent<Rigidbody2D>();

        attackCounter = attackTime;

        if (quest == null || !quest.initialized)
            ActiveQuestBG.gameObject.SetActive(false);
        else
            UpdateQuestUI();

        // set date
        GameController.Instance.calendar.actualMonth = GameController.Instance.calendar.Months[0]; // primo mese
        GameController.Instance.calendar.today = GameController.Instance.calendar.actualMonth.days[29]; // last day

    }

    public void SetScene(SceneDetails currscene)
    {
        prevScene = currentScene;
        currentScene = currscene;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput == Vector2.zero)
            speed = walkingSpeedDefault;
    }

    private void FixedUpdate()
    {
    }

    public void HandleUpdate()
    {
        if (!teleporting)
            CameraMover.i.HandleUpdate();

        if(hp <= 0)
        {
            StartCoroutine(Die());
        }

        #region related objects carrying
        if (activeBoat != null)
        {
            activeBoat.transform.position = new Vector3(transform.position.x-1, transform.position.y, 0);
        }
        if (transportingAnimal != null)
        {
            transportingAnimal.transform.position = transportPoint.position;
        }
        if(liftingItem != null)
        {
            liftingItem.transform.position = Head.position;
        }
        #endregion

        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("FacingHorizontal", moveInput.x);
            animator.SetFloat("FacingVertical", moveInput.y);
        }

        if(enableDiagonalMovements)
        {
            /*if (moveInput.x != 0 && moveInput.y != 0)
            {
                moveInput.x *= moveLimiter;
                moveInput.y *= moveLimiter;
            }*/ 
            // with the new input system this is already done
        }
        else
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
                moveInput.y = 0;
            else
                moveInput.x = 0;
        }

        if(canMove || !isFishing)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            animator.SetFloat("Speed", moveInput.sqrMagnitude);

            rb.velocity = moveInput * getSpeed();
            rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
        }

        attackPoint.position = new Vector3(transform.position.x + animator.GetFloat("FacingHorizontal"), transform.position.y+animator.GetFloat("FacingVertical"), transform.position.z);
        transportPoint.localPosition = new Vector3(-1*animator.GetFloat("FacingHorizontal"), -1*animator.GetFloat("FacingVertical"), transform.position.z);
    }

    #region bindings
    public void OpenMenu(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            GameController.Instance.OpenState(GameState.Menu);
            playerInput.currentActionMap = playerInput.actions.FindActionMap("UI");
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            GameController.Instance.EvH.Interact();
        }
    }

    public void RunOrWalk(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            speed = runningSpeed;
    }

    public void useWeapon(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (canAttack)
            {
                if (inventory.equipedWeapon != -1)
                {
                    if (inventory.Weapons[inventory.equipedWeapon].item.longDamage == 0) // arma da vicino (spada)
                        StartCoroutine(useWeapon());
                    else
                        StartCoroutine(useBow());
                }
            }
        }
    }

    public void useTool(InputAction.CallbackContext ctx)
    {
        if(ctx.performed && !isFishing)
        {
            if (GameController.Instance.keyUI.isActiveAndEnabled)
            {
                keyToUse.Use(this);
                inventory.Remove(keyToUse);
                closeDoor.Open();
            }
            else
            {
                useItem();
            }
        }
    }

    public void cancelAction(InputAction.CallbackContext _)
    {
        if (GameController.Instance.plantDetailsUI.gameObject.activeSelf)
            GameController.Instance.plantDetailsUI.gameObject.SetActive(false);

        if (activeHorse != null)
            Dismount();
        else if (activeBoat != null)
            activeBoat.Dismount();
        else if (transportingAnimal != null)
        {
            transportingAnimal.GetComponent<BoxCollider2D>().enabled = true;
            transportingAnimal = null;
            animator.SetBool("carrying", false);
        }
    }

    public void switchWeapons(InputAction.CallbackContext _)
    {
        var temp = inventory.equipedWeapon;
        inventory.equipedWeapon = inventory.secondaryWeapon;
        inventory.secondaryWeapon = temp;
        GameController.Instance.hotbar.UpdateItems();
    }

    public void useExtraSlot(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
        {
            if (inventory.extraSlot != null && inventory.extraSlot.item != null)
            {
                print($"using {inventory.extraSlot.item.name}");
                inventory.extraSlot.item.Use(this);
            }
            else
                print("extra slot is empty");
        }
    }
    #endregion

    public void GetDamage(int dmg)
    {
        StartCoroutine(Camera.main.GetComponent<CameraMover>().Shake(.15f, .15f));
        if (animator.GetBool("holdingShield"))
        {
            if (inventory.Shields[inventory.equipedShield].item.Defense < dmg)
            {
                hp -= inventory.Shields[inventory.equipedShield].item.Defense - dmg;
            }
        }
        else
            hp -= dmg;
    }

    public bool TryGetSomething<T>(out T type, Vector3 pos, float radius = 0.1f, LayerMask? layer = null)
    {
        if (layer == null)
            layer = farmingLayer;

        var collider = Physics2D.OverlapCircle(pos, radius, (LayerMask)layer);

        type = default(T);

        if (collider != null && collider.TryGetComponent(out T t))
        {
            type = t;
            return true;
        }
        return false;
    }

    public void ShowPlantDetails()
    {
        GameController.Instance.plantDetailsUI.gameObject.SetActive(true);
    }

    public void Sleep(System.Action onwakeup)
    {
        animator.SetTrigger("sleep");
        StartCoroutine(GameController.Instance.Sleep(onwakeup));
    }

    public IEnumerator Reach(Transform target)
    {
        canMove = false;
        while (Vector3.Distance(transform.position, target.position) > 1)
        {
            animator.SetFloat("speed", speed);
            animator.SetFloat("FaceX", (target.position.x - transform.position.x));
            animator.SetFloat("FaceY", (target.position.y - transform.position.y));

            transform.position = Vector3.MoveTowards(transform.position, transform.position, speed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
        LookAt(target.position);
        canMove = true;
    }

    public void LookAt(Vector3 pos)
    {
        animator.SetFloat("FacingHorizontal", (pos.x - transform.position.x));
        animator.SetFloat("FacingVertical", (pos.y - transform.position.y));
    }

    public IEnumerator Die()
    {
        yield return new WaitForSeconds(.75f);
        GameController.Instance.OpenState(GameState.GameOver);
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

    IEnumerator useWeapon()
    {
        canAttack = false;

        rb.velocity = Vector2.zero;
        canMove = false;
        print(inventory.equipedWeapon);
        if (inventory.equipedWeapon != -1)
            inventory.Weapons[inventory.equipedWeapon].item.Use(this); // trova l'arma e usala
        yield return new WaitForSeconds(.5f);
        canMove = true;

        canAttack = true;
    }

    IEnumerator useBow()
    {
        canAttack = false;

        animator.SetTrigger("Shoot");
        canMove = false;
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        Shoot();

        canAttack = true;
    }

    void useShield()
    {
        print("Using the shield");
        animator.SetBool("holdingShield", true);
        animator.SetTrigger("useShield");
    }

    void useItem()
    {
        if (inventory.equipedTool != -1)
            inventory.Tools[inventory.equipedTool].item.Use(this);
    }

    public IEnumerator DiscoveredNewItem()
    {
        animator.SetTrigger("newItemEmote");
        canMove = false;
        yield return new WaitForSeconds(1.5f);
        canMove = true;
    }

    float getSpeed()
    {
        if (animator.GetBool("isRiding"))
            return ridingSpeed;
        else if (animator.GetBool("isClimbing"))
            return 2.5f;
        //else if (Input.GetKey(KeyCode.LeftShift))
        //    return runningSpeed;
        else if (animator.GetBool("holdingShield"))
            return holdingShieldSpeed;
        else
            return walkingSpeedDefault;
    }

    public Vector3Int GetPointedPosition_vec3int()
    {
        var res = new Vector3Int((int)(transform.position.x + animator.GetFloat("FacingHorizontal")), (int)((transform.position.y + animator.GetFloat("FacingVertical"))-.8f), 0);
        //var res = new Vector3Int((int)(transform.position.x + animator.GetFloat("FacingHorizontal")/2), (int)(transform.position.y + animator.GetFloat("FacingVertical")/2 - .3f)-1, (int)transform.position.z);
        return res;
    }

    public Vector2Int GetPointedPosition_vec2int()
    {
        var res = new Vector2Int((int)(transform.position.x + animator.GetFloat("FacingHorizontal") / 2), (int)(transform.position.y + animator.GetFloat("FacingVertical") / 2 - .3f) - 1);
        return res;
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
        GameController.Instance.isFirstLaunch = data.firstLaunch;
        enableDiagonalMovements = data.enableDiagonalMoves;
        kents = data.kents;
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
        if(quest!=null && quest.goal != null && !quest.initialized)
        {
            if (quest.goal[0].goal == "")
                return;

            // enable quests on hud cuz instead of removing text now i'll turn off the container gameobj.
            ActiveQuestBG.gameObject.SetActive(true);

            // now set text
            questUI.text = quest.goal[0].goal;

            // if is a totType goal, enable fraction
            if(quest.goal[0].goalType == GoalType.BuyTot || quest.goal[0].goalType == GoalType.SellTot || quest.goal[0].goalType == GoalType.GetTot || quest.goal[0].goalType == GoalType.KillTot)
            {
                questUI.alignment = TextAnchor.MiddleLeft;
                questUIfraction.gameObject.SetActive(true);
                questUIfraction.text = $"{quest.goal[0].currentAmount}/{quest.goal[0].RequiredAmount}";
            }
            else
            {
                questUIfraction.gameObject.SetActive(false);
                questUI.alignment = TextAnchor.MiddleCenter;
            }

        }
        else
        {
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
        StartCoroutine(startDialogueWithDelay());
    }

    IEnumerator startDialogueWithDelay()
    {
        if (quest.goal[0].introDialogue.sentences.Length > 0)
        {
            yield return new WaitForSeconds(quest.goal[0].dialogueDelay);
            GameController.Instance.dialogueBox.StartDialogue(quest.goal[0].introDialogue, () =>
            {
                GameController.Instance.state = GameState.FreeRoam;
            });
        }
    }

    public void Attack(Weapon weapon)
    {
        // start animation
        animator.SetTrigger("SwordAttack");
        animator.SetBool("Attacking", false);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, interactableLayer | farmingLayer);

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