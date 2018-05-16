﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planiranje.Models
{
    public class SS_Plan
    {
        [Required]
        public int ID_plan { get; set; }
        [Required]
        public int ID_pedagog { get; set; }
        [Required]
        public string Ak_godina { get; set; }
        [Required]
        public string Naziv { get; set; }
        [Required]
        public string Opis { get; set; }
    }
}