using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShasavicMusicMaker.Command
{
    interface IEditorCommand
    {
        void Execute();   // 実行
        void Undo();      // 元に戻す
    }
}
