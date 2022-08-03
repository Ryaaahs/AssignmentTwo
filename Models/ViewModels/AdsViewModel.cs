﻿using EntityFramework.Models;

namespace AssignmentTwo.Models.ViewModels
{
    public class AdsViewModel
    {
        public Brokerage Brokerage { get; set; }
        public IEnumerable<Advertisement> Advertisements { get; set; }
    }
}
