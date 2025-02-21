using System;
using System.Collections;
using System.Diagnostics;

namespace Sharper.ECS
{
    public class Component
    {
        
        public Component()
        {
            owner = null;
            instanceID = 0;
        }
        public virtual void CopyComponentData(Component reference)
        {

        }
        public readonly UInt64 bitSignaturePlace;
        public Entity owner;
        private int instanceID;
        public int InstanceID
        {
            get { return instanceID; }
            set { instanceID = value; }
        }

        //Target entity is the entity that had this component added
        public void OnAdd(Entity targetEntity, int newInstanceID = 0)
        {
            owner = targetEntity;
            InstanceID = newInstanceID;
        }

        public void PropagateChangeMessage()
        {
            owner.EntityChangedEvent.Invoke(this,EventArgs.Empty);
        }
        public void OnRemove()
        {
            owner = null;
        }
    }
}
