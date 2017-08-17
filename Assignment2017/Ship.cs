using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2017
{
    class Ship
    {
        public Model Model;
        public Matrix[] Transforms;

        public Vector3 Position = Vector3.Zero;

        public Vector3 Velocity = Vector3.Zero;
        private const float VelocityScale = 5.0f;

        public Matrix RotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
        private float rotation;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                float newVal = value;
                while (newVal>=MathHelper.TwoPi)
                {
                    newVal -= MathHelper.TwoPi;
                }
                if (rotation!=value)
                {
                    rotation = value;
                    RotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(rotation);
                }
            }
        }

        public void Update(GamePadState controllerState)
        {
            Rotation -= controllerState.ThumbSticks.Left.X * 0.10f;

            Velocity += RotationMatrix.Forward * VelocityScale * controllerState.Triggers.Right;
        }



    }
}
