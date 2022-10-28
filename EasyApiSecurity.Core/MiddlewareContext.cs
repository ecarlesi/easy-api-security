﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyApiSecurity.Core
{
    public class MiddlewareContext
    {
        public IAuthorizationManager Storage { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }
}