using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;

public abstract class NPCState {
    protected NPCController npc;
    protected NPC_FSM npcFSM;

    public NPCState(NPCController npc, NPC_FSM fsm) { 
        this.npc = npc;
        this.npcFSM = fsm;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract string Exit();
}

public class NPC_FSM {
    protected NPCController npc;
    protected NPCState currentState;
    protected Dictionary<string, NPCState> dictOfStates;

    public NPC_FSM(NPCController npc) {
        this.npc = npc;

        CreateDictionaryOFfStates();
    }

    public virtual void CreateDictionaryOFfStates() { 
        dictOfStates = new Dictionary<string, NPCState>();

        dictOfStates.Add("Idle", new NPCState_Idle(npc, this));
        dictOfStates.Add("Move", new NPCState_Move(npc, this));
    }

    public void ChangeState() {
        string str_NextState = "Idle";
        if (currentState != null) { 
            str_NextState = currentState.Exit();
        }

        NPCState tState = null;
        if (dictOfStates.ContainsKey(str_NextState))
        {
            tState = dictOfStates[str_NextState];

            npc.strState = str_NextState;
        }
        else {
            Debug.LogError("There are no NPCState dict: " + str_NextState);

            npc.strState = "Idle";
        }

        currentState = tState;
        currentState.Enter();
    }

    public void Update() {
        if (currentState != null)
        {
            currentState.Update();
        }
        else {
            ChangeState();
        }
    }
}

public class NPCState_Idle : NPCState {
    public float crntIdleTimer;
    public float idleTimer;

    public NPCState_Idle(NPCController npc, NPC_FSM fsm) : base(npc, fsm) { 
    }

    public override void Enter()
    {
        int idleAnimIndex = npc.GetRandomIdleAnimationIndex();
//        npc.animator.SetInteger("Animation_int", idleAnimIndex);

        crntIdleTimer = 0;
        idleTimer = GetIdleTimer();

        npc.SetMovingActive(false);
    }

    public override void Update() {
        crntIdleTimer += Time.deltaTime;
        if (crntIdleTimer >= idleTimer) {
            npcFSM.ChangeState();
            return;
        }
    }

    public override string Exit()
    {
//        npc.animator.SetInteger("Animation_int", 0);
        return "Move";
    }

    public virtual float GetIdleTimer() {
        return npc.GetIdleTimer();
    }
}

public class NPCState_Move : NPCState { 
    public NPCState_Move(NPCController npc, NPC_FSM fsm) : base(npc, fsm) {
    }

    public override void Enter()
    {
        npc.SetMoveSpeed(false);
        SetDestination();
        npc.SetMovingActive(true);
    }

    public override void Update()
    {
        if (npc.isAtDestination) { 
            npcFSM.ChangeState();
            return;
        }
    }

    public override string Exit()
    {

        return "Idle";
    }

    public virtual void SetDestination() {
        Vector3 point = Vector3.zero;

        string loc = npc.GetRandomLocation();
        if (LocationManager.Instance.RandomPointAtLocation(loc, out point)) { 
            npc.SetDestination(point);
        } else if (npc.RandomPoint(out point)) {
            npc.SetDestination(point);
        }
    }
}

public class NPCController : MonoBehaviour
{
    public Animator animator;

    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] protected Rigidbody m_rigidbody;
    [SerializeField] protected CapsuleCollider capsuleCollider;


    protected NPC_FSM npcFSM;

    [SerializeField] public int idleAnimCount = 0;

    [SerializeField] public float randomPointRadius = 5.0f;
    [SerializeField] protected float arrivingDistance = 0.3f;

    [Header("Speed")]
    [SerializeField] private float runningSpeed = 6.0f;
    [SerializeField] private float walkingSpeed = 3.0f;

    [SerializeField] private string[] availableLocations;

    [Header("Timers")]
    public Vector2 idleTimerMinMax;

    [Header("Set Dynamically")]
    [SerializeField] public string strState;

    protected Vector3 _destinationPoint;
    protected virtual Vector3 destinationPoint {
        get { return _destinationPoint; }
        set { _destinationPoint = value; }
    }

    public Vector3 velocity {
        get {
            if (navMeshAgent.isActiveAndEnabled) { 
                return navMeshAgent.velocity;
            }
            return Vector3.zero;
        }
    }

    public virtual float sqrDistToDestinationPoint {
        get { return (transform.position - destinationPoint).sqrMagnitude; }
    }

    public virtual bool isAtDestination {
        get { return sqrDistToDestinationPoint <= arrivingDistance * arrivingDistance; }
    }

    protected virtual void Awake() {

        SetNpcFSM();
    }

    protected virtual void Update() {

        SetMovingAnimation();
        npcFSM?.Update();

    }

    protected virtual void SetNpcFSM() { 
        npcFSM = new NPC_FSM(this);
    }

    public virtual void SetDestination(Vector3 destinationPoint) {
        this.destinationPoint = destinationPoint;
        if (!navMeshAgent.isOnNavMesh) {
            Debug.Log("Try set Destination for object " + gameObject.name + " which is not on nav mesh");
            return;
        }
        navMeshAgent.SetDestination(destinationPoint);
    }

    protected virtual void SetMovingAnimation() {
        animator.SetFloat("Speed_f", velocity.magnitude);
    }

    public virtual void SetMovingActive(bool value) {
        if (value)
        {
            if (!navMeshAgent.isOnNavMesh) return;
            navMeshAgent.isStopped = false;
        }
        else {
            if (!navMeshAgent.isActiveAndEnabled) return;
            if (!navMeshAgent.isOnNavMesh) return;

            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
        }
    }

    public virtual void SetNavMeshAgentActive(bool value) {
        navMeshAgent.enabled = value;
    }

    public void SetMoveSpeed(bool isRunning) {
        navMeshAgent.speed = isRunning ? runningSpeed : walkingSpeed;
    }

    public int GetRandomIdleAnimationIndex() { 
        return Random.Range(0, idleAnimCount);
    }

    public float GetIdleTimer() {
        return Random.Range(idleTimerMinMax.x, idleTimerMinMax.y);
    }

    public bool RandomPointAtLocation(string location, out Vector3 point) {
        point = Vector3.zero;
        if (!LocationManager.instanceExists) {
            return false;
        }
        
        return LocationManager.Instance.RandomPointAtLocation(location, out point);
    }

    public bool RandomPoint(out Vector3 point) {
        return LocationManager.RandomPoint(transform.position, randomPointRadius, out point);
    }

    public string GetRandomLocation() {
        int randInd = Random.Range(0, availableLocations.Length);
        return availableLocations[randInd];
    }
}
