﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vivius.Repository.Model
{
    internal class RequestChangeEmailEntity
    {
        public string Code { get; set; }
        public string NewEmail { get; set; }
        public string OldEmail { get; set; }
    }
}
