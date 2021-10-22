using System.Collections;
using Pathfinding;
using Player.Enums;
using UnityEngine;
using UnityEngine.Events;

public class NPCController : MonoBehaviour
{
    [Header("Parameters")] 
    [Tooltip("Time the NPC will spend waiting at its target destination")]
    [SerializeField] private float timeWaiting = 3f;
    [Tooltip("Amount of time to wait before we compute a new path to follow")]
    [SerializeField] private float pathCalculationDelay = 1f;
    [Tooltip("The distance from a waypoint at which the NPC will start looking for the next waypoint")]
    [SerializeField] private float waypointDistance = 0.1f;

    [Header("References")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Seeker seeker;
    [SerializeField] private Rigidbody2D rigidbody;

    [HideInInspector]
    public UnityEvent<NPCType> npcDestroyed;
    
    private float _speed;
    private Vector3 _target;
    private Vector3 _exit;

    private bool _reachedTarget;
    private int _currentWaypoint;
    private Path _path;
    
    private NPCType _npcType;

    private void Start()
    {
        StartCoroutine(FindPathCoroutine());
    }

    private void FixedUpdate()
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
        var direction = (immediateTarget - transform.position).normalized;
        var movement = Vector3.MoveTowards(transform.position, immediateTarget, _speed * Time.fixedDeltaTime);
        transform.position = movement;

        // Flip the sprite if we're moving towards "negative x"
        sprite.flipX = direction.x < 0f;

        // Start moving to the next waypoint if we're close enough
        if (Vector3.Distance(transform.position, immediateTarget) < waypointDistance)
            _currentWaypoint++;
    }

    public void SetType(NPCType type)
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
                    npcDestroyed ??= new UnityEvent<NPCType>();
                    npcDestroyed.Invoke(_npcType);
                
                    Destroy(gameObject);
                }
                // If we're at the shop, go to the exit
                else
                {
                    yield return new WaitForSeconds(timeWaiting);
                    
                    _target = _exit;
                    _reachedTarget = false;
                }
            }
            
            seeker.StartPath(transform.position, _target, OnPathComplete);
            yield return new WaitForSeconds(pathCalculationDelay);
        }
    }
}
