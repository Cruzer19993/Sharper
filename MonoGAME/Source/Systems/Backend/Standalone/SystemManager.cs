using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGAME.Systems.Backend.Management
{
    public class SystemManager
    {
        public EventHandler<float> UpdateEvent;
        public SystemManager()
        {
            activeSystems = new List<ECSSystem>();
        }
        public List<ECSSystem> activeSystems;

        public void InitializeSystems()
        {
            foreach (ECSSystem x in activeSystems)
            {
                x.Initialize();
            }
        }

        public void AttachSystem(ECSSystem system)
        {
            if (!activeSystems.Exists(x => x.GetType() == system.GetType()))
            {
                activeSystems.Add(system);
                system.Initialize();
            }
            else Debug.WriteLine($"[INTERNAL ERR] Couldn't add system {system.GetType().Name} to current level because another instance is activated already");
        }
    }
}
