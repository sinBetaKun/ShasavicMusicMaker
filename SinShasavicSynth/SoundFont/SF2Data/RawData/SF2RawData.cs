using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Sdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData
{
    internal record SF2RawData
    {
        public readonly SF2SdtaChunk Sdta;
        public readonly SF2PdtaChunk Pdta;

        public SF2RawData(string path)
        {
            using (BinaryReader reader = new(File.OpenRead(path)))
            {
                string groupID = Encoding.ASCII.GetString(reader.ReadBytes(4));

                if (groupID != "RIFF")
                    throw new InvalidDataException("This soundfont is invalid.");

                reader.BaseStream.Seek(4, SeekOrigin.Current);
                string riffType = Encoding.ASCII.GetString(reader.ReadBytes(4));

                if (riffType != "sfbk")
                    throw new InvalidDataException("This soundfont is invalid.");

                string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

                if (id != "LIST")
                    throw new InvalidDataException("INFO chunk isn't found.");

                uint sizeOfINFO = reader.ReadUInt32();

                id = Encoding.ASCII.GetString(reader.ReadBytes(4));

                if (id != "INFO")
                    throw new InvalidDataException("INFO chunk isn't found.");

                reader.BaseStream.Seek(sizeOfINFO - 4, SeekOrigin.Current);

                Sdta = new(reader);
                Pdta = new(reader);
            }
        }
    }
}
