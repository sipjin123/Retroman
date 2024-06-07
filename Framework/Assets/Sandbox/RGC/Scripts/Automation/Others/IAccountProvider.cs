﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.RGC
{
    public interface IAccountProvider
    {
        void Initialize();
        void LoginAsGuest();
        void LoginAsFB();
        void FetchData();
    }
}
