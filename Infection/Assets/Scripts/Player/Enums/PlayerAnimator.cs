using UnityEngine;

namespace Player.Enums
{
    public static class PlayerAnimator
    {
        public static int Running => Animator.StringToHash("Running");
        
        public static int Grounded => Animator.StringToHash("Grounded");
        
        public static int Jump => Animator.StringToHash("Jump");
    }
}