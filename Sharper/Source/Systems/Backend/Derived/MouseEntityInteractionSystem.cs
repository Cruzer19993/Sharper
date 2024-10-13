using Microsoft.Xna.Framework;
using Sharper.Components;
using Sharper.ECS;
using Sharper.Systems.Backend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Systems.Backend
{
    public class MouseEntityInteractionSystem : ECSSystem
    {
        private static MouseEntityInteractionSystem instance;
        private Transform currentCameraTransform;
        private Camera currentCamera;
        public MouseEntityInteractionSystem() : base() 
        {
            if(instance == null)
            {
                instance = this;
            }
        }
        private int pixelsPerSprite;
        private float cameraZoomLevel;
        public static MouseEntityInteractionSystem Instance
        {
            get { return instance; }
            private set { }
        }
        public EventHandler<EntityClickedEventArgs> EntityClicked;
        public override void Initialize()
        {
            matchingComponentTypes = new Type[] { typeof(Transform), typeof(MouseInteractable)};
            base.Initialize();
            RenderingSystem.CameraChangedEvent += OnCameraChange;
        }
        public Entity GetEntityUnderCursor()
        {
            if (currentCamera == null) return null;
            /* mousepos = resolution
             * gamepos = resolution/pps
             * realtexturesize = vec2(X,Y) * camerazoom (ex. 16pps, zoom:2, realTexSize = 32 units;
             *
             */
            //translate mousePos to grid position
            Vector2 currentMousePos = InputSystem.MousePosition();
            float realPPS = pixelsPerSprite * cameraZoomLevel; //calculate current real pps
            //convert camera transform to game window cooridnates
            Vector2 absoluteCursorPosition = currentMousePos + new Vector2(currentCameraTransform.position.X,currentCameraTransform.position.Y);
            absoluteCursorPosition *= (1 / cameraZoomLevel);
            Entity entityUnderCursor = null;
            for (int i = 0; i < realArrayEntityCount; i++)
            {
                MouseInteractable entityInteractableComponent = matchingEntities[i].GetComponent<MouseInteractable>();
                bool entityUsingOwnHitbox = entityInteractableComponent.useOwnHitboxSize;
                float hitboxWidth = entityUsingOwnHitbox ? entityInteractableComponent.hitboxSize.X : pixelsPerSprite;
                float hitboxHeight = entityUsingOwnHitbox ? entityInteractableComponent.hitboxSize.Y : pixelsPerSprite;
                Vector3 entityPos = matchingEntities[i].GetComponent<Transform>().Position;
                if (entityPos.X < absoluteCursorPosition.X &&
                    entityPos.X + hitboxWidth > absoluteCursorPosition.X &&
                    entityPos.Y < absoluteCursorPosition.Y &&
                    entityPos.Y + hitboxHeight > absoluteCursorPosition.Y)
                {
                    Debug.WriteLine(entityPos);
                    entityUnderCursor = matchingEntities[i];
                }
            }
            return entityUnderCursor;
        }

        void OnCameraChange(object seder, CameraChangedEventArgs args)
        {
            currentCamera = args.newCamera;
            cameraZoomLevel = currentCamera.cameraZoom;
            currentCameraTransform = args.newCameraEntity.GetComponent<Transform>();
        }

        public override void GameUpdate()
        {
            if(EntityClicked == null) return;
            foreach(MouseButton button in Enum.GetValues(typeof(MouseButton)))
            {
                if (InputSystem.ButtonPressed(button))
                {
                    Entity entUderCursor = GetEntityUnderCursor();
                    if(entUderCursor != null)
                    {
                        EntityClicked.Invoke(this, new EntityClickedEventArgs(entUderCursor.entityID,button));
                    }
                }
            }
        }

        public override void EntityUpdate(Entity target)
        {
        }

        public override void Start()
        {
            pixelsPerSprite = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
        }

    }
    public class EntityClickedEventArgs : EventArgs
    {
        public EntityClickedEventArgs(int entityID, MouseButton button)
        {
            this.entityID = entityID;
            this.button = button;
        }
        public int entityID;
        public MouseButton button;
    }
}
