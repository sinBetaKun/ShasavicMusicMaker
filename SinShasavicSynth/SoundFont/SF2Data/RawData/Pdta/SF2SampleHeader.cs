using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// sf2のshdrチャンク内のヘッダー用レコード
    /// 詳細:https://www.utsbox.com/?p=2090#shdr%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal record SF2SampleHeader
    {
        /// <summary>
        /// サンプルの名前。終端文字(\0)含め20文字の文字列
        /// </summary>
        readonly string name;

        /// <summary>
        /// 音声波形データの開始位置(単位：サンプル)
        /// </summary>
        readonly uint start;

        /// <summary>
        /// 音声波形データの終了位置(単位：サンプル)
        /// </summary>
        readonly uint end;

        /// <summary>
        /// 音声波形データのループ開始位置(単位：サンプル)
        /// </summary>
        readonly uint loopstart;

        /// <summary>
        /// 音声波形データのループ終了位置(単位：サンプル)
        /// </summary>
        readonly uint loopend;

        /// <summary>
        /// 音声波形データのサンプリングレート
        /// </summary>
        readonly uint sampleRate;

        /// <summary>
        /// 音声波形データのオリジナルの音程
        /// <br/>
        /// 60の時、音声波形はC4(中央のド、261.62Hz)の音程で録音された波形であることを示す
        /// </summary>
        readonly byte originalKey;

        /// <summary>
        /// オリジナルの音程に対しての補正(単位cent)
        /// </summary>
        readonly sbyte correction;

        /// <summary>
        /// 音声波形データのタイプ(下記)が1の場合…0、2(4)の場合…左(右)サンプルのSFSampleHeaderへのインデックス
        /// </summary>
        readonly ushort sampleLink;

        /// <summary>
        /// 音声波形データのタイプ
        /// <br/>
        /// 1…モノラル、2…右サンプル、4…左サンプル、8…リンクサンプル(未定義)
        /// </summary>
        readonly ushort type;

        public static int Size => 46;

        public SF2SampleHeader(BinaryReader reader)
        {
            name = Encoding.ASCII.GetString(reader.ReadBytes(20)).TrimEnd('\0');
            start = reader.ReadUInt32();
            end = reader.ReadUInt32();
            loopstart = reader.ReadUInt32();
            loopend = reader.ReadUInt32();
            sampleRate = reader.ReadUInt32();
            originalKey = reader.ReadByte();
            correction = reader.ReadSByte();
            sampleLink = reader.ReadUInt16();
        }
    }
}
