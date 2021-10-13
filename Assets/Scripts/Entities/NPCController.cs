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

    Animator animator;

    bool done = false;
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
        if (type == NPCType.Talking || type == NPCType.ComplexQuestGiver || type == NPCType.QuestGiver || type == NPCType.TalkAndGive)
            TriggerDialogue();

        if(type == NPCType.TalkAndGive)
        {
            player.inventory.Add(drop);
        }
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

                if (Name == "Asbjorn" && !done)
                {
                    GameController.Instance.storyController.talkedWithAsbjorn = true;
                    StartCoroutine(GameController.Instance.storyController.AsbjornDialogueDone());
                }

                else if (Name == "Ulfr" && !done && GameController.Instance.storyController.talkedWithAsbjorn)
                    StartCoroutine(GameController.Instance.storyController.UlfrDialogueDone());

                GameController.Instance.state = GameState.FreeRoam;

                if(Player.i.quest != null)
                    Player.i.quest.goal[0].NPCTalked(this);

                done = true;

                if (Name == "Ulfr" && !GameController.Instance.storyController.talkedWithAsbjorn)
                    done = false;
            });
        }
        else
        {
            if(dialoguesQueue.Count>0)
            {
                GameController.Instance.dialogueBox.StartDialogue(new Dialogue(dialoguesQueue[0]), () => { GameController.Instance.state = GameState.FreeRoam; Player.i.quest.goal[0].NPCTalked(this); });
                dialoguesQueue.RemoveAt(0);
            }
            else
            {
                GameController.Instance.dialogueBox.StartDialogue(dialoguesAfterWork[i], () => { GameController.Instance.state = GameState.FreeRoam; Player.i.quest.goal[0].NPCTalked(this); });
                i++;
                if (i >= dialoguesAfterWork.Count)
                    i = 0;
            }
        }
    }

    void PointToPlayer() // nessuno ha capito perchè non cambia i float dell'animator ma okk
    {
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
        }
    }

    private void Update()
    {
        if (GameController.Instance.state != GameState.FreeRoam)
        {
            return;
        }

        if (HP <= 0)
            onDie();

        if (Player.i.isInRange(this))
            PointToPlayer();

        if(isAttacking)
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
        }
    }

    // Animation event
    public void AttackEnd()
    {
        GameController.Instance.player.TakeDamage(attackDamage);
    }

    float DistanceFromPlayer()
    {
        return Vector2.Distance(GameController.Instance.player.transform.position, transform.position);
    }

    void onDie()
    {
        if (GameController.Instance.player.quest != null)
        {
            GameController.Instance.player.quest.goal[0].EnemyKilled(this);
        }
    }

    public void onTalk()
    {
        if(GameController.Instance.player.quest != null)
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
        FindObjectOfType<Player>().QuestsContainer.Add(quest);
        quest.isActive = true;
        FindObjectOfType<Player>().quest = quest;
        GameController.Instance.state = GameState.FreeRoam;
    }

    public void Attack()
    {
        animator.SetTrigger("SwordAttack");
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

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ShowSignal()
    {

    }
}
