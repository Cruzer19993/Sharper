using System;
using System.Collections;
using System.Diagnostics;

namespace Sharper.ECS
{
    /* bitshifting for component signatures, each component should have a unique signature, this is used for systems to check if an entity has the required components
     * bit vs component table
     * 0 - Component
     * 1 - Transform
     * 2 - EntityRenderer
     * 3 - Camera
     * 4 - Collider
     * 5 - MouseInteractable
     * 6 - SimpleMover
     * 7 - GUIRect
     * 8 - GUIRenderer
     * 9 - GUIButton
     * 10 - GUIInputBox
     * 11 - GUIText
     * 12 - nothing
     */
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
        public Entity owner;
        private int instanceID;
        protected static BitArray componentSignature = new BitArray(256);
        public static BitArray Signature
        {
            get { return componentSignature; }
            protected set { componentSignature = value; }
        }
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
