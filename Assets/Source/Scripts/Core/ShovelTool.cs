using UnityEngine;

namespace Core
{
    public sealed class ShovelTool : MonoBehaviour
    {
        public void HandleShovelMode()
        {
            GameCondition.ChangeShovelModeCondition();
        }
    }
}