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

    public bool storyCharacter = false;
    [ConditionalField(nameof(storyCharacter))] Dialogue onStoryDialogue;

    public int HP = 10;
    [ConditionalField(nameof(type), true, NPCType.Enemy)] public bool canBeDamaged = false;

    [ConditionalField(nameof(type), false, NPCType.TalkAndGive, NPCType.ComplexQuestGiver)] [SerializeField] ItemBase drop;

    Animator animator;

    // IA variables
    bool isDistanceCheck = false;
    float timeLeft = 0.3f;
    bool isAttacking = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (type == NPCType.Enemy)
            canBeDamaged = true;
    }

    public void Interact(Player player)
    {
        GameController.Instance.ActiveNPC = this;
        if (type == NPCType.Talking)
            TriggerDialogue();
        else if (type == NPCType.QuestGiver)
            OpenQuestWindow();
        else if(type == NPCType.TalkAndGive)
        {
            TriggerDialogue();
            player.inventory.Add(drop);
        }
        else if(type == NPCType.ComplexQuestGiver)
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (storyCharacter && FindObjectOfType<Player>().quest != null)
            GameController.Instance.dialogueBox.StartDialogue(onStoryDialogue, ()=> { });
        else
            GameController.Instance.dialogueBox.StartDialogue(dialogue, () => {
                if(type == NPCType.ComplexQuestGiver || type == NPCType.TalkAndGive)
                {
                    Player.i.inventory.Add(drop);
                }
                if(type == NPCType.ComplexQuestGiver || type == NPCType.QuestGiver)
                {
                    Player.i.QuestsContainer.Add(quest);
                }
            });
        //onTalk();
    }

    private void Update()
    {
        if (GameController.Instance.state != GameState.FreeRoam)
        {
            return;
        }

        if (HP <= 0)
            onDie();

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
            GameController.Instance.player.quest.goal.EnemyKilled(this);
        }
    }

    public void onTalk()
    {
        if(GameController.Instance.player.quest != null)
        {
            GameController.Instance.player.quest.goal.NPCTalked(this);
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
