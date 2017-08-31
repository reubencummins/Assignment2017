using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2017
{
    struct Asteroid
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;

        public bool isActive;

        public void Update(float delta)
        {
            position += direction * speed * GameConstants.AsteroidSpeedAdjustment * delta;
            if (position.X > GameConstants.PlayfieldSizeX)
                position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (position.X < -GameConstants.PlayfieldSizeX)
                position.X += 2 * GameConstants.PlayfieldSizeX;
            if (position.Y > GameConstants.PlayfieldSizeY)
                position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (position.Y < -GameConstants.PlayfieldSizeY)
                position.Y += 2 * GameConstants.PlayfieldSizeY;
        }
    }
}
