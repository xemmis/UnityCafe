
namespace Core.Dialogue
{
    using System;
    using Models.Npc;

    [Serializable]
    public sealed class OrderPreferenceConfig
    {
        public FoodPrefer Prefer;
        public DialogueTree[] HintTrees;
    }
}
