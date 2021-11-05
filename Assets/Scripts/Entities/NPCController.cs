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
    ComplexQuestGiver
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

    [SerializeField] int expDrop = 0;

    Animator animator;

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
    }

    private void Awake()
    {
        if (type == NPCType.Enemy)
            canBeDamaged = true;
    }

    public void Interact(Player player)
    {
        GameController.Instance.ActiveNPC = this;
        if (type != NPCType.Enemy)
            TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if(repeatOperation || !done)
        {
            GameController.Instance.dialogueBox.StartDialogue(dialogue, () =>
            {
                if (type == NPCType.ComplexQuestGiver || type == NPCType.TalkAndGive)
                {
                    if (drop != null)
                        Player.i.inventory.Add(drop);
                }
                if (type == NPCType.ComplexQuestGiver || type == NPCType.QuestGiver)
                {
                    Player.i.QuestsContainer.Add(quest);
                }

                if (type == NPCType.Enemy)
                {
                    
                }

                done = true;
                GameController.Instance.storyController.NPCTalked(this);
                GameController.Instance.state = GameState.FreeRoam;
            });
        }
        else
        {
            if(dialoguesQueue.Count>0)
            {
                GameController.Instance.dialogueBox.StartDialogue(new Dialogue(dialoguesQueue[0]), () =>
                {
                    GameController.Instance.state = GameState.FreeRoam;
                    if(Player.i.quest.goal != null)
                        Player.i.quest.goal[0].NPCTalked(this);
                });
                dialoguesQueue.RemoveAt(0);
            }
            else
            {
                GameController.Instance.dialogueBox.StartDialogue(dialoguesAfterWork[i], () =>
                {
                    GameController.Instance.state = GameState.FreeRoam;
                    if(Player.i.quest.goal != null)
                        Player.i.quest.goal[0].NPCTalked(this);
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

        animator.SetTrigger("Hurt");
        /*if(type != NPCType.Enemy)
            animator.SetTrigger("Hurt");
        else
        {
            isAttacking = true;
        }*/
        /*var miniNum = Instantiate(numPrefab, transform);
        miniNum.GetComponent<UnityEngine.UI.Text>().text = dmg.ToString();*/

        if(type == NPCType.Enemy)
        {
            getKnockBack();
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
