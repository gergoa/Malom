using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malom.Model;

namespace Malom.Persistence
{
    public interface IFileHandler
    {
        TableData? OpenFile(string path);
        bool SaveFile(TableData gameState, string path);
    }
}
