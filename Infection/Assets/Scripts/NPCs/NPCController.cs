using System.Collections;
using Player.Enums;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Parameters")] 
    [Tooltip("Time the NPC will spend waiting at its target destination")]
    [SerializeField] private float timeWaiting = 3f;
    [SerializeField] private float modelVerticalOffset = -0.21f;
    
    [Header("References")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    
    private float _speed;
    private Vector3 _target;
    private Vector3 _exit;

    void Update()
    {
        var distance = Vector3.Distance(transform.position, _target);
        
        animator.SetBool(NPCAnimator.Running, distance > 0f);
        
        if (_speed == 0f || _target == Vector3.zero)
            return;
        
        // Target reached
        if (distance <= 0f)
        {
            // Destroy the NPC
            if (_target == _exit)
            {
                Destroy(gameObject);
            }
            // If we're at the shop, go to the exit
            else
            {
                StartCoroutine(GoToExitCoroutine());
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);

        sprite.flipX = (_target - transform.position).x < 0f;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetTargetPosition(Vector3 target)
    {
        _target = target;
        _target.y += modelVerticalOffset;
    }

    public void SetExitPosition(Vector3 exit)
    {
        _exit = exit;
    }

    private IEnumerator GoToExitCoroutine()
    {
        while (_target != _exit)
        {
            yield return new WaitForSeconds(timeWaiting);
            _target = _exit;
        }
    }
}
