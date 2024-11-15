﻿using Sharper.ECS;
using Sharper.Systems.Backend;
using System;
using System.Timers;

namespace Sharper.Components.GUI
{
    public class GUIButton : Component
    {
        public GUIButton()
        {
            activateButton = MouseButton.Left;
            componentSignature.Set(9, true);
        }
        public Timer clickAnimTimer;
        public event EventHandler OnClick;
        public MouseButton activateButton;
        public void Clicked()
        {
            if (OnClick != null) OnClick.Invoke(this, EventArgs.Empty);
        }
    }
}
