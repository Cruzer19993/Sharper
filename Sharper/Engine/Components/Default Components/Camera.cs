using Microsoft.Xna.Framework;
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
            m_isMainCamera = false;
            m_cameraZoom = 2f;
            m_backbufferColor = Color.Black;
        }
        public bool m_isMainCamera;
        public float m_cameraZoom;
        public Color m_backbufferColor;
    }
}
