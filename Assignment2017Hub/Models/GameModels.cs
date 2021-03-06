﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Assignment2017Hub.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class HiScore
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public ApplicationUser Owner { get; set; }
    }

    public class UserAchievement
    {
        public ApplicationUser User { get; set; }
        public Achievement Achieve { get; set; }
        public bool Complete { get; set; }
    }

    public class Player
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public string ScreenName { get; set; }
        public List<UserAchievement>;
    }

    public class GameContext : DbContext
    {
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<HiScore> HiScores { get; set; }
        public void Seed()
        {
            //seed database with default scores & achievements
            //not here?
        }
    }
}