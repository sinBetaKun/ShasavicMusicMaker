using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal record PresetCollection
    {
        readonly Preset[] presets;

        public PresetCollection(SF2RawData raw)
        {
            presets = new Preset[raw.Pdta.Phdr.Headers.Length];

            for (ushort i = 0; i < presets.Length; i++)
            {
                presets[i] = new(i, raw);
            }
        }
    }
}
