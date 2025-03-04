﻿using Microsoft.Xna.Framework;
using Sharper.Components.GUI;

namespace Sharper.ECS
{
    public class Transform : Component
    {
        public Transform(Transform other)
        {
            parent = other.parent;
            Position = other.Position;
            Rotation = other.Rotation;
            Scale = other.Scale;
            lastPosition = other.lastPosition;
        }
        public Transform()
        {
            parent = null;
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
            lastPosition = Position;
        }
        public Vector3 lastPosition;
        public Transform parent;
        public Rectangle renderRect;
        public Vector3 position;
        public Vector3 Position
        {
            get
            {
                if (parent == null)
                {
                    return position;
                }
                else return parent.position + position;
            }
            set { position = value; }
        }
        public Quaternion Rotation;
        public Vector3 Scale;

        public void GetChild()
        {

        }

        public override void CopyComponentData(Component reference)
        {
            if(reference is Transform t)
            {
                this.parent = t.parent;
                this.Position = t.Position;
                this.Rotation = t.Rotation;
                this.Scale = t.Scale;
                this.lastPosition = t.lastPosition;
            }
        }
        public void ClearParent()
        {
            this.parent = null;
        } 
        public void SetParent(Transform parent)
        {
            this.parent = parent;
        }
    }
}
