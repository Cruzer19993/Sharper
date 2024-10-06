﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGAME.Components;
using MonoGAME.ECS;
using MonoGAME.Systems.Backend;
using System;
using System.Diagnostics;
using Vector3 = Microsoft.Xna.Framework.Vector3;


namespace MonoGAME.Systems.GameSystems
{
    public class SimpleMovement : ECSSystem
    {
        public float scrollSpeed = 100f;
        float deltaMovement = 0.0f;
        public SimpleMovement() { }
        public override void Initialize()
        {
            matchingComponentTypes = new Type[] { typeof(SimpleMover), typeof(Transform),typeof(Camera)};
            base.Initialize();
        }
        public override void GameUpdate()
        {

        }
        public override void EntityUpdate(Entity target)
        {
            if (realArrayEntityCount == 0) return;
            Vector3 dir = Vector3.Zero;
            if (InputSystem.IsKeyDown(Keys.D)) dir -= Vector3.UnitX;
            if (InputSystem.IsKeyDown(Keys.A)) dir += Vector3.UnitX;
            if (InputSystem.IsKeyDown(Keys.W)) dir += Vector3.UnitY;
            if (InputSystem.IsKeyDown(Keys.S)) dir -= Vector3.UnitY;
            Vector3.Normalize(dir);
            deltaMovement += frameTime;
            float currentCameraZoom = target.GetComponent<Camera>().cameraZoom;
            target.GetComponent<Camera>().cameraZoom += InputSystem.ScrollDelta * scrollSpeed * frameTime;
            //target.GetComponent<Camera>().cameraZoom += InputSystem.ScrollDelta * scrollSpeed * frameTime;
            target.GetComponent<Transform>().Position += dir * target.GetComponent<SimpleMover>().movementSpeed * frameTime;
        }

        public override void Start()
        {

        }
    }
}
