﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Contracts.Services
{
    public interface IWpfDialogService
    {
        string? OpenFile(string filter);
    }
}
