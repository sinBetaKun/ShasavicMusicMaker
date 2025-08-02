using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class SampleHeader_b
    {
        /// <summary>
        /// 音声波形データの開始位置(単位：サンプル)
        /// </summary>
        public uint Start { get; init; }

        /// <summary>
        /// 音声波形データの終了位置(単位：サンプル)
        /// </summary>
        public uint End { get; init; }

        /// <summary>
        /// 音声波形データのループ開始位置(単位：サンプル)
        /// </summary>
        public uint Loopstart { get; init; }

        /// <summary>
        /// 音声波形データのループ終了位置(単位：サンプル)
        /// </summary>
        public uint Loopend { get; init; }

        /// <summary>
        /// 音声波形データのサンプリングレート
        /// </summary>
        public uint SampleRate { get; init; }

        /// <summary>
        /// 音声波形データのオリジナルの音程
        /// <br/>
        /// 60の時、音声波形はC4(中央のド、261.62Hz)の音程で録音された波形であることを示す
        /// </summary>
        public byte OriginalKey { get; init; }

        /// <summary>
        /// オリジナルの音程に対しての補正(単位cent)
        /// </summary>
        public sbyte Correction { get; init; }

        public SampleHeader_b(SampleHeader header)
        {
            Start = header.Start;
            End = header.End;
            Loopstart = header.Loopstart;
            Loopend = header.Loopend;
            SampleRate = header.SampleRate;
            OriginalKey = header.OriginalKey;
            Correction = header.Correction;
        }
    }
}
