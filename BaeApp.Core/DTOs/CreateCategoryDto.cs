﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.DTOs
{
    public class CreateCategoryDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
