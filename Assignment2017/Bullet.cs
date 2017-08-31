using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2017
{
    struct Bullet
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;

        public bool isActive;

        public void Update(float delta)
        {
            position += direction * speed * GameConstants.BulletSpeedAdjustment * delta;
            if (position.X > GameConstants.PlayfieldSizeX ||
                position.X < -GameConstants.PlayfieldSizeX ||
                position.Y > GameConstants.PlayfieldSizeY ||
                position.Y < -GameConstants.PlayfieldSizeY)
                isActive = false;
        }
    }
}
