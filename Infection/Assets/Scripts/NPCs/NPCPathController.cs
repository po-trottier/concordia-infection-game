using System.Collections;
using System.Linq;
using NPCs;
using Pathfinding;
using Player.Enums;
using UnityEngine;
using UnityEngine.Events;

public class NPCPathController : MonoBehaviour
{
    [Header("Parameters")] 
    [Tooltip("Time the NPC will spend waiting at its target destination")]
    [SerializeField] private float timeWaiting = 3f;
    [Tooltip("Amount of time to wait before we compute a new path to follow")]
    [SerializeField] private float pathCalculationDelay = 1f;
    [Tooltip("The distance from a waypoint at which the NPC will start looking for the next waypoint")]
    [SerializeField] private float waypointDistance = 0.1f;
    
    [Header("Score Parameters")] 
    [Tooltip("The NPCs that should remove points when they leave the scene")]
    [SerializeField] private NPCType[] npcsRemovePoint;

    [Header("References")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Seeker seeker;
    [SerializeField] private NPCInteractionController interaction;

    [HideInInspector] public UnityEvent<NPCType> npcDestroyed;
    
    private float _speed;
    private Vector3 _target;
    private Vector3 _exit;

    private bool _reachedTarget;
    private int _currentWaypoint;
    private Path _path;
    
    private NPCType _npcType;

    private ScoreManager _scoreManager;

    private void Start()
    {
        _scoreManager = FindObjectOfType<ScoreManager>();

        if (_scoreManager == null)
            throw new UnityException("No Score Manager was found");
        
        StartCoroutine(FindPathCoroutine());
    }

    private void Update()
    {
        // If values were not set yet then wait
        if (_path == null || _speed == 0f || _target == Vector3.zero)
            return;
        
        _reachedTarget = _currentWaypoint >= _path.vectorPath.Count;
        
        animator.SetBool(NPCAnimator.Running, !_reachedTarget);
        
        if (_reachedTarget)
            return;

        var immediateTarget = _path.vectorPath[_currentWaypoint];
        
        // Move the player in the direction the pathfinder gives us
        var movement = Vector3.MoveTowards(transform.position, immediateTarget, _speed * Time.deltaTime);
        transform.position = movement;
        
        // Flip the sprite if we're moving towards "negative x"
        sprite.flipX = (immediateTarget - transform.position).x < 0f;

        // Start moving to the next waypoint if we're close enough
        if (Vector3.Distance(transform.position, immediateTarget) < waypointDistance)
            _currentWaypoint++;
    }

    public Vector3 GetTargetPosition()
    {
        return _target;
    }

    public Vector3 GetExitPosition()
    {
        return _exit;
    }

    public void SetTypeNPC(NPCType type)
    {
        _npcType = type;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetTargetPosition(Vector3 target)
    {
        _target = target;
    }

    public void SetExitPosition(Vector3 exit)
    {
        _exit = exit;
    }

    public void OnNPCScared()
    {
        _target = _exit;
    }

    private void OnPathComplete(Path p)
    {
        if (p.error)
            throw new UnityException(p.errorLog);
        
        _path = p;
        _currentWaypoint = 0;
    }

    private IEnumerator FindPathCoroutine()
    {
        while (seeker.IsDone())
        {
            if (_reachedTarget)
            {
                // If we're at the exit, destroy the NPC
                if (_target == _exit)
                {
                    if (npcsRemovePoint.Contains(_npcType))
                        _scoreManager.UpdateLives(-1);
                    
                    npcDestroyed ??= new UnityEvent<NPCType>();
                    npcDestroyed.Invoke(_npcType);
                
                    Destroy(gameObject);
                }
                // If we're at the shop, go to the exit
                else
                {
                    interaction.SetWaiting(true);
                    
                    yield return new WaitForSeconds(timeWaiting);
                    
                    interaction.SetWaiting(false);
                    
                    _target = _exit;
                    _reachedTarget = false;
                }
            }

            seeker.StartPath(transform.position, _target, OnPathComplete);
            yield return new WaitForSeconds(pathCalculationDelay);
        }
    }
}
