﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamProfile.Models
{
    public class Feature
    {
        public int FeatureId { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public bool Equipped { get; set; }
    }
}
