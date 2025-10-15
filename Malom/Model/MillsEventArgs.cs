using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malom.Model
{
    public class MillsEventArgs
    {
        public string NextAction { get; set; }
        public MillsEventArgs(string nextAction)
        {
            NextAction = nextAction;
        }
    }
}
