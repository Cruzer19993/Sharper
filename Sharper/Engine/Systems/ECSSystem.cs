using Sharper.ECS;
using Sharper.Systems.Backend.Management;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sharper.Systems
{
    public class MatchingPattern
    {
        public MatchingPattern(params Type[] patternTypes)
        {
            neededTypes = patternTypes;
            Random rand = new Random();
            patternSignature = string.Join(",",patternTypes.Select(x => x.FullName)).GetHashCode();
        }
        public MatchingPattern()
        {
            neededTypes = new Type[0];
            Random rand = new Random();
            patternSignature = -1;
        }
        public Type[] neededTypes;
        public int patternSignature;
    }

    public abstract class ECSSystem
    {

        public ECSSystem()
        {
            isInitialized = false;
            stopUsingRemovedEntities = true;
            entityManager = null;
            systemManager = null;
            frameTime = 0;
        }
        public bool isInitialized = false;
        public bool stopUsingRemovedEntities = true;
        protected EntityManager entityManager;
        protected SystemManager systemManager;
        protected float frameTime;
        protected Dictionary<MatchingPattern, List<Entity>> matchingEntities = new Dictionary<MatchingPattern, List<Entity>>();
        protected List<MatchingPattern> patterns = new List<MatchingPattern>();
        public bool IsEntityMatchingSystem(Entity entity, out List<MatchingPattern> matchedPatterns) //check if entity matches components for this system to use
        {
            bool AnyMatches = false;
            bool matches = true;
            matchedPatterns = new List<MatchingPattern>();
            foreach(MatchingPattern pattern in patterns)
            {
                matches = true;
                for(int i=0; i < pattern.neededTypes.Length; i++)
                {
                    if (!entity.HasComponent<Component>(pattern.neededTypes[i]))
                    {
                        matches = false;
                        break;
                    }
                }
                if (matches)
                {
                    matchedPatterns.Add(pattern);
                    AnyMatches = true;
                }
            }
            return AnyMatches;
        }

        protected void AddMatchingPatterns(params MatchingPattern[] newPatterns)
        {
            for(int i = 0; i < newPatterns.Length; i++)
            {
                patterns.Add(newPatterns[i]);
                matchingEntities.Add(newPatterns[i], new List<Entity>());
            }
        }

        public void AttachEntity(Entity entity) //adds a new entity created at compile-time.
        {
            if (!IsEntityMatchingSystem(entity, out List<MatchingPattern> patterns)) return;
            foreach (MatchingPattern pattern in patterns)
            {
                matchingEntities[pattern].Add(entity);
                OnEntityAttached(entity, pattern);
            }
        }

        protected virtual void OnNewEntityRegistered(object sender, EntityEventArgs args) //attaches runtime created objects.
        {
            //When new entity is detected, check if it matches the components this system needs and add it to matching entities
            Entity newEntity = SceneManager.CurrentScene.entityManager.GetEntityWithID(args.entityID);
            if (IsEntityMatchingSystem(newEntity, out List<MatchingPattern> patterns))
            {
                foreach (MatchingPattern pattern in patterns)
                {
                    if (matchingEntities.ContainsKey(pattern))
                    {
                        matchingEntities[pattern].Add(newEntity);
                        OnEntityAttached(newEntity, pattern);
                    }
                    else
                    {
                        matchingEntities.Add(pattern, new List<Entity>());
                        matchingEntities[pattern].Add(newEntity);
                        OnEntityAttached(newEntity, pattern);
                    }
                }
            }
        }
        protected virtual void OnEntityRemoved(object sender, EntityEventArgs args)
        {
            if (!stopUsingRemovedEntities) return;
            Entity target = entityManager.GetEntityWithID((int)args.entityID);
            foreach(MatchingPattern pattern in matchingEntities.Keys)
            {
                if(matchingEntities[pattern].Contains(target)) matchingEntities[pattern].Remove(target);
            }
        }
        public virtual void Initialize()
        {
            entityManager = SceneManager.CurrentScene.entityManager;
            systemManager = SceneManager.CurrentScene.systemManager;
            entityManager.NewEntityRegisteredEvent += OnNewEntityRegistered;
            entityManager.EntityRemovedEvent += OnEntityRemoved;
            systemManager.UpdateEvent += GetUpdateEvent;
            lock (entityManager.entities)
            {
                for(int i = 0; i < entityManager.entities.Count; i++)
                {
                    AttachEntity(entityManager.entities[i]);
                }
            }
            isInitialized = true;
            Debug.WriteLine($"[INFO] System {this.GetType().FullName} has been initialized");
            Start();
        }
        private void GetUpdateEvent(object sender, float _frameTime)
        {
            frameTime = _frameTime;
            GameUpdate();
            foreach(MatchingPattern pattern in matchingEntities.Keys)
            {
                foreach(Entity entity in matchingEntities[pattern])
                {
                    OnEntityUpdate(entity, pattern);
                }
            }

        }
        public abstract void OnEntityDetached(Entity target);
        public abstract void OnEntityAttached(Entity target, MatchingPattern pattern);
        public abstract void GameUpdate();
        public abstract void OnEntityUpdate(Entity target, MatchingPattern pattern);
        public abstract void Start();
    }
}
