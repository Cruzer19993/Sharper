using MonoGAME.Components.GUI;
using MonoGAME.ECS;

namespace MonoGAME.Systems.Backend.GUI
{
    public class GUIRenderingSystem : ECSSystem
    {
        public GUIRenderingSystem () 
        {
            matchingComponentTypes = new System.Type[]
            {
                typeof(GUIRect),
                typeof(GUIRenderer)
            };
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Start()
        {
        }
        public override void GameUpdate()
        {
        }
        public override void EntityUpdate(Entity target)
        {
        }
    }
}
