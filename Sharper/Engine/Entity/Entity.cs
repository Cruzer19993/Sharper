using Sharper.Systems.Backend.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Sharper.ECS
{
    public class Entity
    {
        public Entity(Entity copyFrom)
        {
            dontRemoveWhenUnused = copyFrom.dontRemoveWhenUnused;
            usedBySystemCount = copyFrom.usedBySystemCount;
            componentsOccupiedSize = copyFrom.componentsOccupiedSize;
            entityID = -1;
            entityName = copyFrom.entityName;
        }

        public Entity(string _entityName, params Type[] newComponents)
        {
            entityName = _entityName;
            componentsOccupiedSize = 0;
            foreach (Type componentType in newComponents)
            {
                AddComponent<Component>(componentType);
            }
        }

        private static int COMPONENTS_ARRAY_MAX_SIZE = 64;
        private int componentsOccupiedSize;
        private int usedBySystemCount = 0;
        private string entityName;
        private string entityTag;
        private bool dontRemoveWhenUnused = false;
        private Dictionary<Type, List<Component>> components = new Dictionary<Type, List<Component>>(8);

        public EventHandler EntityChangedEvent;
        public int entityID;
        public int ComponentsOccupiedSize
        {
            get { return componentsOccupiedSize; }
        }

        public bool DontRemoveWhenUnused
        {
            get { return dontRemoveWhenUnused; }
            set { dontRemoveWhenUnused = value; }
        }

        public int UsedBySystemCount
        {
            get { return usedBySystemCount; }
            set { usedBySystemCount = value; OnSystemUsageChange(); }
        }

        public string EntityName
        {
            get { return entityName; }
        }

        public string EntityTag
        {
            get { return entityTag; }
            set { entityTag = value; }
        }

        protected void OnSystemUsageChange()
        {
            if (usedBySystemCount == 0 && !dontRemoveWhenUnused)
            {
                SceneManager.CurrentScene.entityManager.RemoveEntity(entityID);
            }
        }
        public T GetComponent<T>(int instanceID) where T : Component
        {
            if (components.ContainsKey(typeof(T)))
                return (T)components[typeof(T)].Find(x => x.InstanceID == instanceID);
            else return null;
        }
        public T GetComponent<T>(Type typeToMatch = null) where T : Component
        {
            Type key = typeToMatch ?? typeof(T);
            if (components.TryGetValue(key, out var componentList) && componentList.Count > 0)
            {
                return componentList[0] as T;
            }
            return null;
        }

        public int GetNumberOfComponentInstances<T>(Type typeToMatch = null) where T : Component
        {
            return components[typeToMatch ?? typeof(T)].Count;
        }
        public void AddComponent<T>(ref Component existingComponent) where T : Component
        {
            int newInstanceID = 0;
            if (HasComponent<T>())
            {
                newInstanceID = GetNumberOfComponentInstances<T>();
            }
            if (components.ContainsKey(typeof(T)))
            {
                components[existingComponent.GetType()].Add(existingComponent);
            }
            else
            {
                components.Add(existingComponent.GetType(), new List<Component>());
                components[existingComponent.GetType()].Add(existingComponent);
            }
            existingComponent.OnAdd(this, newInstanceID);
        }
        public void AddComponent<T>(Type typeToMatch = null) where T : Component
        {
            int newInstanceID = 0;
            if (HasComponent<T>())
            {
                newInstanceID = GetNumberOfComponentInstances<T>();
            }
            var newComponent = typeToMatch != null ? (Component)Activator.CreateInstance(typeToMatch) : (Component)Activator.CreateInstance<T>();
            if (components.ContainsKey(typeof(T)))
            {
                components[newComponent.GetType()].Add(newComponent);
            }
            else
            {
                components.Add(newComponent.GetType(), new List<Component>());
                components[newComponent.GetType()].Add(newComponent);
            }
            newComponent.OnAdd(this, newInstanceID);
        }

        public bool HasComponent<T>(Type typeToMatch = null) where T : Component
        {
            return components.ContainsKey(typeToMatch ?? typeof(T));
        }

        public void RemoveComponent(ref Component component)
        {
            if (components.ContainsKey(component.GetType()))
            {
                components[component.GetType()].Remove(component);
            }
        }

        public void RemoveComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (components.ContainsKey(type) && components[type].Count > 0)
            {
                Component componentToDeletion = components[type][0];
                components[typeof(T)].Remove(componentToDeletion);
            }
        }

        public T[] GetAllComponentsOfType<T>() where T : Component
        {
            return components[typeof(T)].ConvertAll(x => (T)x).ToArray();
        }

        public Type[] GetTypesOfActiveComponents()
        {
            return components.Keys.ToArray();
        }
    }
}