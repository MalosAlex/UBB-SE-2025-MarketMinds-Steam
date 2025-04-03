﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SteamProfile.Models
{
    public class UserProfile
    {
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public DateTime LastModified { get; set; }
    }
} 