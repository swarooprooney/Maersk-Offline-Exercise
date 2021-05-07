using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Configuration
{
    public class DBConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string JobDatabase { get; set; } = "JobDatabase";
    }
}
