using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShasavicMusicMaker
{
    class OriginalEventSample
    {
        public delegate void OnFruitSelectedEventHandler(OnFruitSelectedEventArgs e);
        public event OnFruitSelectedEventHandler OnFruitSelected;
        // デリゲートの引数を定義
        public class OnFruitSelectedEventArgs : EventArgs
        {
            public string strFruit;
        }
    }
}
