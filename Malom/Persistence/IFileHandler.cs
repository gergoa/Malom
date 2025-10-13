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
        Model.GameModel? OpenFile(string path);
        bool SaveFile(Model.GameModel gameState, string path);
    }
}
