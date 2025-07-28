using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ShasavicMusicMaker.ScoreData.Event.Abstract;
using ShasavicMusicMaker.ScoreData.NoteData;

namespace ShasavicMusicMaker.ScoreData.Event.Performer
{
    internal record ChordonymChangeEvent : ScoreEventBase
    {
        public Chordonym Chordonym { get; init; }

        public override Grid EventLabel { get; init; }

        public ChordonymChangeEvent(Chordonym chordonym)
        {
            Chordonym = chordonym;

        }
    }
}
