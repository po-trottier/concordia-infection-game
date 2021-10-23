using Common;
using Player.Enums;
using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        UpdatedAllowedActionsNPC(other, CollisionState.Enter);
    }

    private void OnTriggerExit(Collider other)
    {
        UpdatedAllowedActionsNPC(other, CollisionState.Exit);
    }

    private void UpdatedAllowedActionsNPC(Collider other, CollisionState state)
    {
        Debug.Log("NPC near player: " + state);
    }
}
