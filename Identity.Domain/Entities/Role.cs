﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public Role() {
            this.Id = Guid.NewGuid();
        }
    }
}
