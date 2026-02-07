using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Sdta
{
    internal class Sm24Chunk
    {
        public uint Size { get; init; }

        static string ID => "sm24";

        public Sm24Chunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            Size = reader.ReadUInt32();
            reader.BaseStream.Seek(Size, SeekOrigin.Current);
        }
    }
}
