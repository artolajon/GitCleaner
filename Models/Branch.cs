using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitCleaner.Models
{
    internal class Branch
    {
        public string Name { get; set; }
        public int CommitsAhead { get; set; }
        public int CommitsBehind { get; set; }
    }

}
