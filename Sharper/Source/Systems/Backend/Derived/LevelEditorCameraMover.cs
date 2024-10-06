using Sharper.Components;
using Sharper.Components.GUI;
using Sharper.ECS;
using Sharper.Systems;
using Sharper.Systems.Backend;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;

namespace Sharper
{
    public class LevelEditorCameraMover : ECSSystem
    {
        Vector2 originPoint = Vector2.Zero;
        Vector2 currentMousePos;
        Vector2 lastMousePos;
        Vector2 mouseMovementVector;
        int pps;
        Camera currentCamera;
        bool resetCameraToCenter;
        //Runs only once before Start
        public override void Initialize()
        {
            matchingComponentTypes = new System.Type[]
            {
                        typeof(Transform),
                        typeof(Camera),
                        typeof(SimpleMover)
            };
            base.Initialize();
            RenderingSystem.CameraChangedEvent += CameraZoomChanged;
        }
        //Runs once after Initialize
        public override void Start()
        {
            pps = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
        }
        //Updates every frame
        public override void GameUpdate()
        {
            if (InputSystem.KeyPressed(Microsoft.Xna.Framework.Input.Keys.B))
            {
                resetCameraToCenter = true;
            }
            if (InputSystem.ButtonDown(MouseButton.Middle))
            {
                lastMousePos = currentMousePos;
                currentMousePos = InputSystem.MousePosition();
                if (originPoint == Vector2.Zero)
                    originPoint = currentMousePos;
                if(lastMousePos == currentMousePos)
                {
                    originPoint = currentMousePos;
                }
                mouseMovementVector = currentMousePos - lastMousePos;
                mouseMovementVector.Normalize();
            }
            else
            {
                originPoint = Vector2.Zero;
                lastMousePos = Vector2.Zero;
                currentMousePos = Vector2.Zero;
                mouseMovementVector = Vector2.Zero;
            }
        }
        //Applies changed to target entities
        public override void EntityUpdate(Entity target)
        {
            if (lastMousePos == currentMousePos)
            {
                originPoint = currentMousePos;
            }
            else
            {
                target.GetComponent<Transform>().position -= new Vector3(mouseMovementVector.X, mouseMovementVector.Y, 0f)*(1/currentCamera.cameraZoom);
            }
            if (resetCameraToCenter)
            {
                target.GetComponent<Transform>().position = Vector3.Zero;
                resetCameraToCenter = false;
            }
            float scrollDelta = InputSystem.ScrollDelta;
            target.GetComponent<Camera>().cameraZoom += scrollDelta * 20f * frameTime;
            target.GetComponent<Camera>().cameraZoom = Math.Clamp(target.GetComponent<Camera>().cameraZoom, 0.05f, 5f);
            if (scrollDelta < 0)
            {
                //TODO: Translate mouse position to world position  
                Vector3 mousePosVector = new Vector3(currentMousePos.X,currentMousePos.Y,0f);
                target.GetComponent<Transform>().position = Vector3.Lerp(target.GetComponent<Transform>().position, mousePosVector, 0.1f);
            }
        }

        public void CameraZoomChanged(object sender, CameraChangedEventArgs args)
        {
            currentCamera = args.newCamera;
        }
    }
}
