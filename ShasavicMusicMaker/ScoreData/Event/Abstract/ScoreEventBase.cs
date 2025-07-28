using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShasavicMusicMaker.ScoreData.Event.Abstract
{
    abstract record ScoreEventBase
    {
        public int Mea { get; init; }

        public int Tick { get; init; }

        public abstract Grid EventLabel { get; init; }
    }
}
