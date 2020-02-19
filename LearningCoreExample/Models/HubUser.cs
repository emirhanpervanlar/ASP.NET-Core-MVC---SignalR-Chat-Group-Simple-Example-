using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningCoreExample.Models
{
    public class HubUser
    {
        public string userName { get; set; }
        public List<string> groupList = new List<string>();
    }
}
