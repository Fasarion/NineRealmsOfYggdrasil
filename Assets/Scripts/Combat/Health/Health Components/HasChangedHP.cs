using Unity.Entities;

namespace Health
{
    public struct HasChangedHP : IComponentData, IEnableableComponent
    {
        public HasChangedHP(float amount)
        {
            Amount = amount;
        }
        
        public float Amount;
    }
}