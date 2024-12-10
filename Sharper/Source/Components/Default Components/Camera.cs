using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Components
{
    public class Camera : Component
    {
        public Camera()
        {
            isMainCamera = false;
            cameraZoom = 2f;
        }
        public bool isMainCamera;
        public float cameraZoom;
    }
}
