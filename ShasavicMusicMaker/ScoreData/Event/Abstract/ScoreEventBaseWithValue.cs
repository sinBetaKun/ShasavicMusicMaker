using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShasavicMusicMaker.ScoreData.Event.Abstract
{
    abstract record ScoreEventBaseWithValue : ScoreEventBase
    {
        public int Value { get; init; }
    }
}
