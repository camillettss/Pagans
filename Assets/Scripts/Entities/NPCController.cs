using MyBox;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public enum NPCType
{
    Talking,
    QuestGiver,
    Enemy,
    TalkAndGive,
    ComplexQuestGiver,
    Librarian
}

[RequireComponent(typeof(Animator))]
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

    [SerializeField] bool WalkingCharacter = false;
    [ConditionalField(nameof(WalkingCharacter))] [SerializeField] List<WalkStep> steps = new List<WalkStep>();
    int actualStep = 0;
    bool isWalking = false;
    public bool canMove = true;
    public bool isTalking = false;
    NPCController talkingWith=null;
    public int socialityLevel = 5;
    float speed = 2.5f;
    float checkTimer = 0f;

    public CityDetails triggeredCity;

    List<NPCController> alreadyCheckedNPCs;

    private void Start()
    {
        animator = GetComponent<Animator>();
        quest.giver = this;
        done = false;
        storyGizmosDone = false;
        canMove = true;
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
        canMove = false;
        GameController.Instance.ActiveNPC = this;

        if (type == NPCType.Librarian)
            GameController.Instance.OpenState(GameState.Library);

        else if (type != NPCType.Enemy)
            TriggerDialogue();
    }

    public void TriggerDialogue()
    {
        if (!storyGizmosDone) {
            foreach (var pdialogue in particleDialogues)
            {
                if (pdialogue.questName == Player.i.quest.title.GetLocalizedString())
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

    [System.Obsolete("unmaintained function, use NPCController.LookAt() instead.", true)]
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

    private void FixedUpdate()
    {

        if (type == NPCType.Enemy)
            return;

        if (GameController.Instance.state != GameState.FreeRoam)
        {
            unshowSignal();
            return;
        }

        if (HP <= 0)
            onDie();

        if (Player.i.isInRange(this) && type != NPCType.Enemy && !done)
            ShowSignal();
        else
            unshowSignal();
    }

    [System.Obsolete("algoritmo obsoleto, usa A* per spostarti.", false)]
    IEnumerator MoveBy(WalkStep step, int tolerance = 0)
    {
        Vector3 target = new Vector3(transform.position.x+step.step.x, transform.position.y+step.step.y);
        isWalking = true;
        while (Vector3.Distance(transform.position, target) > tolerance)
        {
            if (canMove)
            {
                if (animator != null)
                {
                    animator.SetFloat("speed", 1);
                    animator.SetFloat("FaceX", (target.x - transform.position.x));
                    animator.SetFloat("FaceY", (target.y - transform.position.y));
                }

                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
            }
            else
            {
                if (animator.GetFloat("speed") > 0)
                    animator.SetFloat("speed", 0);
            }

            if (Vector3.Distance(Player.i.transform.position, transform.position) < 2.8f)
            {
                speed = 1.5f;
                lookAt(Player.i.transform.position);
            }
            else if (speed < 2) speed = 2.5f;

            /*if(checkTimer >= 1f)
            {
                foreach (var collider in Physics2D.OverlapCircleAll(transform.position, .5f))
                {
                    if (collider.TryGetComponent(out NPCController npc) && npc != this && npc.WalkingCharacter && !npc.isTalking)
                    {
                        if (UnityEngine.Random.Range(0, socialityLevel + 1) == 2)
                        {
                            print($"go talk with:{npc.name}");
                            TalkWith(npc);
                        }
                    }
                }
                checkTimer = 0f;
            }
            else
            {
                checkTimer += Time.deltaTime;
            }*/

            yield return new WaitForFixedUpdate();
        }
        animator.SetFloat("speed", 0);
        yield return new WaitForSeconds(step.pause);

        actualStep++; // next waypoint
        if (actualStep >= steps.Count)
            actualStep = 0;

        isWalking = false;
    }

    public IEnumerator MoveTo(Vector3 pos, Action onEnd, int tolerance = 1)
    {
        canMove = false;
        while(Vector3.Distance(transform.position, pos) > tolerance)
        {
            animator.SetFloat("speed", 1);
            animator.SetFloat("FaceX", (pos.x - transform.position.x));
            animator.SetFloat("FaceY", (pos.y - transform.position.y));

            transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }
        onEnd?.Invoke();
    }

    void TalkWith(NPCController npc)
    {
        // set bool
        isTalking = true;
        npc.isTalking = true;
        npc.canMove = false;
        // set person
        talkingWith = npc;
        npc.talkingWith = this;

        StartCoroutine(MoveTo(npc.transform.position, () =>
        {
            npc.canMove = false;
            canMove = false;

            npc.lookAt(transform.position);
            lookAt(npc.transform.position);

            StartCoroutine(stopTalkingTimer(socialityLevel));
        }));
    }

    IEnumerator stopTalkingTimer(int time)
    {
        yield return new WaitForSeconds(time);

        // reset other npc
        talkingWith.canMove = true;
        talkingWith.isTalking = false;
        talkingWith.talkingWith = null;

        // reset urself
        isTalking = false;
        talkingWith = null;
        canMove = true;
    }

    void lookAt(Vector3 pos)
    {
        animator.SetFloat("FaceX", (pos.x - transform.position.x));
        animator.SetFloat("FaceY", (pos.y - transform.position.y));
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
        questWindow.titleText.text = quest.title.GetLocalizedString();
        questWindow.DescText.text = quest.description.GetLocalizedString();
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

[System.Serializable]
internal class WalkStep
{
    public Vector2 step;
    public float pause;
}
