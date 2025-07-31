using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal record PresetZone
    {
        /// <summary>
        /// ジェネレータ
        /// </summary>
        public readonly Dictionary<GeneratorType, ushort> Gens;
        
        /// <summary>
        /// 
        /// </summary>
        public readonly Instrument? Inst;

        public PresetZone(IEnumerable<SF2Gen> gens, SF2RawData raw)
        {
            Gens = [];

            foreach (SF2Gen gen in gens)
            {
                if (gen.Oper < 256)
                {
                    if (Enum.IsDefined(typeof(GeneratorType), (byte)gen.Oper))
                    {
                        Gens.Add((GeneratorType)gen.Oper, gen.Amount);
                    }
                }
            }

            if (Gens.TryGetValue(GeneratorType.Instrument, out ushort instIndex))
            {
                Inst = new(instIndex, raw);
            }
        }
    }
}
