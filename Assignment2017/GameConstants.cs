using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2017
{
    class GameConstants
    {
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 16000f;
        public const float PlayfieldSizeY = 12500f;

        public const int NumAsteroids = 10;
        public const float AsteroidMinSpeed = 100f;
        public const float AsteroidMaxSpeed = 300f;
        public const float AsteroidSpeedAdjustment = 5.0f;

        public const float AsteroidBoundingSphereScale = 0.95f;
        public const float ShipBoundingSphereScale = 0.5f;

        public const int NumBullets = 30;
        public const float BulletSpeedAdjustment = 100.0f;

        public const int ShotPenalty = 1;
        public const int DeathPenalty = 100;
        public const int WarpPenalty = 50;
        public const int KillBonus = 25;
    }
}
