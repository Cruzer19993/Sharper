using Sharper.Components;
using Sharper.ECS;
using System;
using Microsoft.Xna.Framework;

namespace Sharper.Helpers
{
    public static class EntityHelper
    {
        public static Entity CreateCamera(Vector3 position, float cameraZoom = 1f, bool isMainCamera = false)
        {
            Entity CameraEntity = new Entity("Camera", new Type[] { typeof(Camera), typeof(Transform) });
            CameraEntity.GetComponent<Transform>().Position = position;
            CameraEntity.GetComponent<Camera>().isMainCamera = isMainCamera;
            CameraEntity.GetComponent<Camera>().cameraZoom = cameraZoom;
            return CameraEntity;
        }
        public static Entity CreateCameraMovable(Vector3 position, float cameraZoom = 1f, bool isMainCamera = false)
        {
            Entity CameraEntity = new Entity("Camera", new Type[] { typeof(Camera), typeof(Transform),typeof(SimpleMover) });
            CameraEntity.DontRemoveWhenUnused = true;
            CameraEntity.GetComponent<Transform>().Position = position;
            CameraEntity.GetComponent<Camera>().isMainCamera = isMainCamera;
            CameraEntity.GetComponent<Camera>().cameraZoom = cameraZoom;
            CameraEntity.GetComponent<SimpleMover>().movementSpeed = 20f;
            return CameraEntity;
        }

        public static Entity CreateGridTile(Vector3 position)
        {
            Entity gridTile = new Entity("Grid Tile", new Type[] { typeof(Transform), typeof(MouseInteractable), typeof(SpriteRenderer) });
            SpriteRenderer spriteRenderer = gridTile.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite.atlasX = 0;
            spriteRenderer.sprite.atlasY = 1;
            spriteRenderer.spriteColor = Color.White;
            gridTile.GetComponent<Transform>().position = position;
            return gridTile;
        }
    }
}
