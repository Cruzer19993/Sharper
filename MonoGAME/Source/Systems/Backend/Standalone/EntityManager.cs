using MonoGAME.ECS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGAME.Systems.Backend.Management
{
    public class EntityManager
    {
        public event EventHandler<EntityEventArgs> EntityComponentsChangedEvent;
        public event EventHandler<EntityEventArgs> NewEntityRegisteredEvent;
        public event EventHandler<EntityEventArgs> EntityRemovedEvent;
        public EntityManager()
        {
            entities = new SortedList<int, Entity>();
        }

        public SortedList<int,Entity> entities;


        //if HasAny is true than this returns entity if it has any of the components
        public Entity[] GetEntitiesWithComponents(params Type[] types)
        {
            //Very dirt, please run once on initialization for systems, and then manage adding and removing entities individually to systems.
            List<Entity> matchingEntities = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                bool passed = true;
                for (int j = 0; j < types.Length; j++)
                {
                    if (!entities[i].HasComponent<Component>(types[j]))
                    {
                        passed = false;
                        break;
                    }
                }
                if (passed)
                {
                    matchingEntities.Add(entities[i]);
                    passed = true;
                }
            }
            return matchingEntities.ToArray();
        }

        //This function returns every entity with at least one component from params.
        public Entity[] GetEntitiesWithPartialComponents(Type[] requiredTypes, params Type[] optionalTypes)
        {
            //Very dirt, please run once on initialization for systems, and then manage adding and removing entities individually to systems.
            List<Entity> requiredEntities = GetEntitiesWithComponents(requiredTypes).ToList();
            List<Entity> matchingEntities = new List<Entity>();
            for (int i = 0; i < requiredEntities.Count; i++)
            {
                for (int j = 0; j < optionalTypes.Length; j++)
                {
                    if (requiredEntities[i].HasComponent<Component>(optionalTypes[j]))
                    {
                        matchingEntities.Add(requiredEntities[i]);
                        break;
                    }
                }
            }
            return matchingEntities.ToArray();
        }

        public Entity GetEntityWithID(int entityID)
        {
            if (entities.ContainsKey(entityID))
            {
                return entities[entityID];
            }
            else return null;
        }

        public void ReleaseEntity(int entityID) //Makes the ECS systems stop using this object
        {
            EntityRemovedEvent.Invoke(this, new EntityEventArgs(entityID));
        }

        public void RemoveEntity(int entityID)//called by entity when it stops being used by any system.
        {
            entities.Remove(entityID);
        }

        public void RegisterEntity(ref Entity entity)
        {
            int newEntityID = entities.Count;
            entity.entityID = newEntityID;
            entities.Add(newEntityID, entity);

            // Use null-conditional operator to invoke the event if it's not null
            NewEntityRegisteredEvent?.Invoke(this, new EntityEventArgs(newEntityID));
        }

    }
    public class EntityEventArgs : EventArgs
    {
        public EntityEventArgs(int id)
        {
            entityID = id;
        }
        public int entityID;
    }
}
