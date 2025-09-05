using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// sf2のshdrチャンク内のヘッダー用クラス
    /// 詳細:https://www.utsbox.com/?p=2090#shdr%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal class SampleHeader
    {
        /// <summary>
        /// サンプルの名前。終端文字(\0)含め20文字の文字列
        /// </summary>
        readonly string name;

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

        /// <summary>
        /// 音声波形データのタイプ(下記)が1の場合…0、2(4)の場合…左(右)サンプルのSFSampleHeaderへのインデックス
        /// </summary>
        public ushort SampleLink { get; init; }

        /// <summary>
        /// 音声波形データのタイプ
        /// <br/>
        /// 1…モノラル、2…右サンプル、4…左サンプル、8…リンクサンプル(未定義)
        /// </summary>
        public ushort Type { get; init; }

        public static int Size => 46;

        public SampleHeader(BinaryReader reader)
        {
            name = Encoding.ASCII.GetString(reader.ReadBytes(20)).TrimEnd('\0');
            Start = reader.ReadUInt32();
            End = reader.ReadUInt32();
            Loopstart = reader.ReadUInt32();
            Loopend = reader.ReadUInt32();
            SampleRate = reader.ReadUInt32();
            OriginalKey = reader.ReadByte();
            Correction = reader.ReadSByte();
            SampleLink = reader.ReadUInt16();
            Type = reader.ReadUInt16();
        }
    }
}
