using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment2017
{
    public class Player
    {
        internal Asteroid[] asteroidList = new Asteroid[GameConstants.NumAsteroids];
        internal Bullet[] bulletList = new Bullet[GameConstants.NumBullets];
        internal Ship ship = new Ship();

        internal GamePadState lastState;

        internal int score;
        Random random = new Random();

        public Player()
        {
            ResetAsteroids();
        }
        
        private void ResetAsteroids()
        {
            float xStart;
            float yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY;
                asteroidList[i].position = new Vector3(xStart, yStart, 0.0f);
                double angle = random.NextDouble() * 2 * Math.PI;
                asteroidList[i].direction.X = -(float)Math.Sin(angle);
                asteroidList[i].direction.Y = (float)Math.Cos(angle);
                asteroidList[i].speed = GameConstants.AsteroidMinSpeed + (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed;
                asteroidList[i].isActive = true;
            }
        }

        internal void Update(GameTime gameTime)
        {
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ship.Position += ship.Velocity;
            ship.Velocity *= 0.95f;

            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (asteroidList[i].isActive)
                {
                    asteroidList[i].Update(timeDelta);
                }
            }

            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                if (bulletList[i].isActive)
                {
                    bulletList[i].Update(timeDelta);
                }
            }
        }

        public void ShootBullet()
        {
            for (int i = 0; i < GameConstants.NumBullets; i++)
            {
                if (!bulletList[i].isActive)
                {
                    bulletList[i].direction = ship.RotationMatrix.Forward;
                    bulletList[i].speed = GameConstants.BulletSpeedAdjustment;
                    bulletList[i].position = ship.Position + (200 * bulletList[i].direction);
                    bulletList[i].isActive = true;
                    score -= GameConstants.ShotPenalty;
                    return;
                }
            }
        }

        public void WarpToCenter()
        {
            ship.Position = Vector3.Zero;
            ship.Velocity = Vector3.Zero;
            ship.Rotation = 0.0f;
            ship.isActive = true;
            score -= GameConstants.WarpPenalty;
        }
        
        public bool CheckForShipAsteroidCollision(float shipRadius, float asteroidRadius)
        {
            if (ship.isActive)
            {
                BoundingSphere shipSphere = new BoundingSphere(ship.Position, shipRadius * GameConstants.ShipBoundingSphereScale);

                for (int i = 0; i < asteroidList.Length; i++)
                {
                    if (asteroidList[i].isActive)
                    {
                        BoundingSphere b = new BoundingSphere(asteroidList[i].position, asteroidRadius * GameConstants.AsteroidBoundingSphereScale);
                        if (b.Intersects(shipSphere))
                        {
                            //destroy ship here
                            ship.isActive = false;
                            score -= GameConstants.DeathPenalty;
                            asteroidList[i].isActive = false;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool CheckForBulletAsteroidCollision(float bulletRadius, float asteroidRadius)
        {
            for (int i = 0; i < asteroidList.Length; i++)
            {
                if (asteroidList[i].isActive)
                {
                    BoundingSphere asteroidSphere = new BoundingSphere(asteroidList[i].position, asteroidRadius * GameConstants.AsteroidBoundingSphereScale);
                    for (int j = 0; j < bulletList.Length; j++)
                    {
                        if (bulletList[j].isActive)
                        {
                            BoundingSphere bulletSphere = new BoundingSphere(bulletList[j].position, bulletRadius);
                            if (asteroidSphere.Intersects(bulletSphere))
                            {
                                asteroidList[i].isActive = false;
                                bulletList[j].isActive = false;
                                score += GameConstants.KillBonus;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
