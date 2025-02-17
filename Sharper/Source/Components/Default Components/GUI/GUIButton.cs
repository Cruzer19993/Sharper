using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sharper.ECS;
using Sharper.Systems.Backend;
namespace Sharper.Components.GUI
{
    public class GUIButton : Component
    {
        public MouseButton activationButton;
        public EventHandler<EventArgs> buttonClicked;
    }
}
