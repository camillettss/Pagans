using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum NPCType
{
    Talking,
    QuestGiver,
    Enemy,
    TalkAndGive,
    ComplexQuestGiver,
    Librarian
}

public class NPCController : MonoBehaviour, IEntity
{
    public string Name;
    public Dialogue dialogue;

    public NPCType type = NPCType.Talking;
    [ConditionalField(nameof(type), false, NPCType.QuestGiver, NPCType.ComplexQuestGiver)] public Quest quest;

    [ConditionalField(nameof(type), false, NPCType.Enemy)] [SerializeField] int attackDamage;

    public int HP = 10;
    [ConditionalField(nameof(type), true, NPCType.Enemy)] public bool canBeDamaged = false;

    [ConditionalField(nameof(type), false, NPCType.TalkAndGive, NPCType.ComplexQuestGiver)] [SerializeField] ItemBase drop;

    [SerializeField] bool repeatOperation;
    [ConditionalField(nameof(repeatOperation), true)] [SerializeField] List<Dialogue> dialoguesAfterWork;

    [SerializeField] bool storyEntity=false;
    [ConditionalField(nameof(storyEntity))][SerializeField] List<ParticleDialogue> particleDialogues;
    bool storyGizmosDone = false;

    [SerializeField] int expDrop = 0;

    Animator animator;
    Rigidbody2D rb;

    public bool done = false;
    bool showingSignal = false;
    int i = 0;

    [HideInInspector] public List<string[]> dialoguesQueue = new List<string[]>();

    // IA variables
    bool isDistanceCheck = false;
    float timeLeft = 0.3f;
    bool isAttacking = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        quest.giver = this;
        done = false;
        storyGizmosDone = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (type == NPCType.Enemy)
            canBeDamaged = true;

        dialogue.Teller = Name;
    }

    public void Interact(Player player)
    {
        GameController.Instance.ActiveNPC = this;

        if (type == NPCType.Librarian)
            GameController.Instance.OpenState(GameState.Library);

        else if (type != NPCType.Enemy)
            TriggerDialogue();
    }

    [System.Obsolete("contenuto in via di sviluppo.", true)]
    public IEnumerator Move(Vector2 vec) // like (2, 4) moves first 2 steps right and then 4 steps up
    {
        var startPos = (Vector2)transform.position;
        var targetPos = new Vector2(transform.position.x + vec.x, transform.position.y + vec.y); // no pathfinding to reach.

        bool isMoving = false;

        while(startPos - targetPos != Vector2.zero)
        {
            if (isMoving)
                yield return 0;

            else
            {
                // scegli se aggiustare X o Y
                if((Mathf.Abs(transform.position.x)-Mathf.Abs(targetPos.x)) < (Mathf.Abs(transform.position.y) - Mathf.Abs(targetPos.y))) // se distanza fra le x maggiore distanza fra y
                {
                    // la distanza fra le x è minore

                    //rb.velocity = moveInput * getSpeed();
                    //rb.MovePosition(rb.position + moveInput * getSpeed() * Time.fixedDeltaTime);
                }
            }
        }
    }

    public void TriggerDialogue()
    {
        if (!storyGizmosDone) {
            foreach (var pdialogue in particleDialogues)
            {
                if (pdialogue.questName == Player.i.quest.title)
                {
                    GameController.Instance.dialogueBox.StartDialogue(pdialogue.differentDialogue, () =>
                    {
                        GameController.Instance.state = GameState.FreeRoam;
                    });
                    storyGizmosDone = true;
                    return;
                }
            }
        }
        if (repeatOperation || !done)
        {
            GameController.Instance.dialogueBox.StartDialogue(dialogue, () =>
            {
                if (type == NPCType.ComplexQuestGiver || type == NPCType.TalkAndGive)
                {
                    if (drop != null)
                        StoryEventHandler.i.AddToInventory(drop);
                }
                if (type == NPCType.ComplexQuestGiver || type == NPCType.QuestGiver)
                {
                    Player.i.QuestsContainer.Add(quest);
                }

                if (type == NPCType.Enemy)
                {

                }

                done = true;
                GameController.Instance.state = GameState.FreeRoam;
            });
        }
        else
        {
            if (dialoguesQueue.Count > 0)
            {
                GameController.Instance.dialogueBox.StartDialogue(new Dialogue(dialoguesQueue[0]), () =>
                {
                    GameController.Instance.state = GameState.FreeRoam;
                    if (Player.i.quest != null)
                        Player.i.quest.goal[0].NPCTalked(this);
                });
                dialoguesQueue.RemoveAt(0);
            }
            else
            {
                GameController.Instance.dialogueBox.StartDialogue(dialoguesAfterWork[i], () =>
                {
                    GameController.Instance.state = GameState.FreeRoam;
                });
                i++;
                if (i >= dialoguesAfterWork.Count)
                    i = 0;
            }
        }
    }

    void PointToPlayer() // nessuno ha capito perchè non cambia i float dell'animator ma okk
    {
        /*
        var n = transform.position; // you
        var p = Player.i.transform.position; // player

        print($"actual face: x: {animator.GetFloat("FaceX")}, y:{animator.GetFloat("FaceY")}");

        if(n.y == p.y)
        {
            // sono allineati orizzontalmente
            if (n.x - p.x > 0)
                animator.SetFloat("FaceX", -1f);
            else
                animator.SetFloat("FaceX", 1f);
        }

        else if(n.x == p.x)
        {
            if (n.y > p.y)
                animator.SetFloat("FaceY", 1f);
            else
                animator.SetFloat("FaceY", -1f);
        }*/
    }

    void unshowSignal()
    {
        if(showingSignal)
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            showingSignal = false;
        }
    }

    public void ShowSignal()
    {
        if (!showingSignal)
        {
            Instantiate(GameController.Instance.keytip_Z, transform);
            showingSignal = true;
        }
    }

    private void Update()
    {
        if (GameController.Instance.state != GameState.FreeRoam)
        {
            unshowSignal();
            return;
        }

        if (HP <= 0)
            onDie();

        if(type==NPCType.Enemy)
        {
            EnemyUpdate();
        }

        if (Player.i.isInRange(this) && type != NPCType.Enemy && !done)
            ShowSignal();
        else
            unshowSignal();

        /*if(isAttacking)
        {
            if(DistanceFromPlayer() < 3.0f)
            {
                if(!isDistanceCheck)
                {
                    //GameController.Instance.ShowMessage("non puoi stare qui.");
                    isDistanceCheck = true;
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                }

                if(timeLeft <= 0.0f)
                {
                    // Attack
                    Attack();
                }
            }
            else
            {
                isDistanceCheck = false;
                timeLeft = 3.0f;
            }
        }*/
    }

    // Enemy update, called by Unity's Update if is an enemy.
    void EnemyUpdate()
    {
        if (Player.i.isInRange(this))
            Attack();
    }

    public void Attack()
    {
        // enemies will be added in 1.5.1
        animator.SetTrigger("Attack");
    }

    // Animation event
    public void AttackEnd()
    {
        GameController.Instance.player.TakeDamage(attackDamage);
    }

    float DistanceFromPlayer()
    {
        return Vector2.Distance(Player.i.transform.position, transform.position);
    }

    void onDie()
    {
        if (GameController.Instance.player.quest != null && Player.i.quest.goal != null)
        {
            GameController.Instance.player.quest.goal[0].EnemyKilled(this);
        }
        Player.i.experience += expDrop;
    }

    public void onTalk()
    {
        if(GameController.Instance.player.quest != null && Player.i.quest.goal != null)
        {
            GameController.Instance.player.quest.goal[0].NPCTalked(this);
        }
    }

    public void OpenQuestWindow()
    {
        var questWindow = GameController.Instance.questWindow;

        questWindow.gameObject.SetActive(true);
        questWindow.titleText.text = quest.title;
        questWindow.DescText.text = quest.description;
        questWindow.goldText.text = $"{quest.goldReward}";
        questWindow.questGiver = this;
        GameController.Instance.state = GameState.Quest;
    }

    public void AcceptQuest()
    {
        GameController.Instance.questWindow.gameObject.SetActive(false);
        Player.i.QuestsContainer.Add(quest);
        quest.isActive = true;
        Player.i.quest = quest;
        GameController.Instance.state = GameState.FreeRoam;
    }

    public virtual void takeDamage(int dmg)
    {
        if(canBeDamaged)
            HP -= dmg;

        if(type == NPCType.Enemy)
        {
            //getKnockBack();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }

        if (HP <= 0)
        {
            Destroy(gameObject);
            onDie();
        }
    }

    IEnumerator getKnockBack()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(Player.i.animator.GetFloat("FacingHorizontal") * 15, Player.i.animator.GetFloat("FacingVertical") * 15);
        yield return new WaitForFixedUpdate();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    IEnumerator getMortalKnockBack()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(Player.i.animator.GetFloat("FacingHorizontal") * 20, Player.i.animator.GetFloat("FacingVertical") * 20);
        yield return new WaitForSeconds(1f);
        onDie();
        Destroy(gameObject);
    }
}

[System.Serializable]
internal class ParticleDialogue
{
    public string questName;
    public Dialogue differentDialogue;
}