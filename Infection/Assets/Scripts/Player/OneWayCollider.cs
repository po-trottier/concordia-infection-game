using Common;
using UnityEngine;

public class OneWayCollider : MonoBehaviour
{
    [SerializeField] private PlatformEffector2D platform;
    
    private PlayerInputController _input;
    
    void Start()
    {
        var player = GameObject.FindWithTag(Tags.Player);
        _input = player.GetComponent<PlayerInputController>();
    }

    void Update()
    {
        if (Mathf.Abs(_input.move.y) <= 0f)
            return;
        
        if (_input.move.y < 0f)
            platform.rotationalOffset = 180f;
        else
            platform.rotationalOffset = 0f;
    }
}
