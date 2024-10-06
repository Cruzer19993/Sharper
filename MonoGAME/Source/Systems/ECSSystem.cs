using MonoGAME.ECS;
using MonoGAME.Systems.Backend.Management;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MonoGAME.Systems
{
    public abstract class ECSSystem
    {

        public ECSSystem()
        {
            isInitialized = false;
            ARRAY_RESIZE_STEP = 128;
            stopUsingRemovedEntities = true;
            entityManager = null;
            systemManager = null;
            frameTime = 0;
            matchingEntities = null;
            RealEntityCount = 0;
            matchingComponentTypes = null;
        }
        public bool isInitialized = false;
        public static int ARRAY_RESIZE_STEP = 128; //How much more space does the system allocate to the array when it runs out of space
        public bool stopUsingRemovedEntities = true;
        protected EntityManager entityManager;
        protected SystemManager systemManager;
        protected float frameTime;
        protected Entity[] matchingEntities;
        protected int realArrayEntityCount = 0;
        protected Type[] matchingComponentTypes;
        protected Type[] partialMatchingComponentTypes;
        protected bool getEntitesMatchingPartially;
        //TODO: change this so that ECS systems use entityIDs instead of direct references to entities (faster)
        public Entity[] MatchingEntities
        {
            get { return matchingEntities; }
            protected set { matchingEntities = value; }
        }
        public int RealEntityCount
        {
            get { return realArrayEntityCount; }
            protected set { realArrayEntityCount = value; }
        }

        public bool IsEntityMatchingSystem(Entity entity) //check if entity matches components for this system to use
        {
            bool matches = true;
            if (getEntitesMatchingPartially)
            {
                for (int i = 0; i < matchingComponentTypes.Length; i++)
                {
                    if (!entity.HasComponent<Component>(matchingComponentTypes[i]))
                    {
                        matches = false;
                        break;
                    }
                }
                if (matches)
                {
                    for(int i=0; i < partialMatchingComponentTypes.Length; i++)
                    {
                        if (entity.HasComponent<Component>(partialMatchingComponentTypes[i]))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < matchingComponentTypes.Length; i++)
                {
                    if (!entity.HasComponent<Component>(matchingComponentTypes[i]))
                    {
                        matches = false;
                        break;
                    }
                }
                return matches;
            }
            return false;
        }

        public void DetachEntity(int entityID) //removes entity from being used by this system, keeping the array without empty spaces
        {
            int foundIndex = -1;
            for (int i = 0; i < realArrayEntityCount; i++)
            {
                if (matchingEntities[i].entityID == entityID)
                {
                    foundIndex = i;
                    break;
                }
            }
            //Check if entity exists in ECSSystem matching entities
            if (foundIndex == -1)
            {
                return;
            }
            //If exists, let it burn.
            if (realArrayEntityCount > 1) //Is there something to plug the hole after removal?
            {
                Entity lastEntity = matchingEntities[realArrayEntityCount - 1];//Grab the last entity
                matchingEntities[foundIndex].UsedBySystemCount--; //decrement entity usage value
                matchingEntities[foundIndex] = lastEntity; //change out the entity
                matchingEntities[realArrayEntityCount - 1] = null; //remove the entity which we used to plug the hole with
            }
            else //ex. singular item case
            {
                matchingEntities[foundIndex].UsedBySystemCount--;
                matchingEntities[foundIndex] = null;
            }
            realArrayEntityCount--;
        }

        public void CheckAndAttachChangedEntity(object sender, EntityEventArgs args)
        {
            Entity entityToCheck = entityManager.GetEntityWithID(args.entityID);
            if (IsEntityMatchingSystem(entityToCheck)) AttachEntity(entityToCheck);
        }

        public void AttachEntity(Entity entity) //adds a new entity and resizes the array if needed by the system.
        {
            //Check for space in the array.
            if (realArrayEntityCount == matchingEntities.Length)
            {
                //No Space? No problem. extend the array once again.
                ExtendArrayByStep();
            }
            matchingEntities[realArrayEntityCount] = entity;
            entity.UsedBySystemCount++;
            realArrayEntityCount++;
            OnNewEntity(entity);
        }
        //called when a new entity is added to matching entities
        public virtual void OnNewEntity(Entity newEntity)
        {

        }

        protected void ExtendArrayByStep()
        {
            Entity[] resizeHolder = matchingEntities;
            int newSize = resizeHolder.Length == 0 ? 1 : resizeHolder.Length * 2;
            matchingEntities = new Entity[newSize];
            resizeHolder.CopyTo(matchingEntities, 0);
            resizeHolder = null;
        }

        protected virtual void OnNewEntityRegistered(object sender, EntityEventArgs args)
        {
            //When new entity is detected, check if it matches the components this system needs and add it to matching entities
            Entity newEntity = SceneManager.CurrentScene.entityManager.GetEntityWithID(args.entityID);
            if (IsEntityMatchingSystem(newEntity))
            {
                AttachEntity(newEntity);
            }
        }
        protected virtual void OnEntityReleased(object sender, EntityEventArgs args)
        {
            if (!stopUsingRemovedEntities) return;
            Entity target = SceneManager.CurrentScene.entityManager.GetEntityWithID(args.entityID);
            DetachEntity(args.entityID);
        }
        public virtual void Initialize()
        {
            entityManager = SceneManager.CurrentScene.entityManager;
            systemManager = SceneManager.CurrentScene.systemManager;
            entityManager.NewEntityRegisteredEvent += OnNewEntityRegistered;
            entityManager.EntityRemovedEvent += OnEntityReleased;
            systemManager.UpdateEvent += GetUpdateEvent;
            entityManager.EntityComponentsChangedEvent += CheckAndAttachChangedEntity;
            if (getEntitesMatchingPartially)
            {
                matchingEntities = SceneManager.CurrentScene.entityManager.GetEntitiesWithPartialComponents(matchingComponentTypes,partialMatchingComponentTypes);
            }
            else
            {
                matchingEntities = SceneManager.CurrentScene.entityManager.GetEntitiesWithComponents(matchingComponentTypes); //Called once on level load.
            }
            realArrayEntityCount = matchingEntities.Length;
            ExtendArrayByStep();
            isInitialized = true;
            Start();
        }
        private void GetUpdateEvent(object sender, float _frameTime)
        {
            frameTime = _frameTime;
            GameUpdate();
            for (int i = 0; i < realArrayEntityCount; i++)
            {
                EntityUpdate(matchingEntities[i]);
            }
        }
        public abstract void GameUpdate();
        public abstract void EntityUpdate(Entity target);
        public abstract void Start();
    }
}
