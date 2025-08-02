using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    ///phdrチャンク内のプリセットヘッダー用クラス
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#phdr%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal class PresetHeader
    {
        /// <summary>
        /// プリセットの名前。終端文字(\0)含め20文字の文字列。
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// MIDIプリセット番号。
        /// </summary>
        public ushort PresetNo { get; init; }

        /// <summary>
        /// MIDIバンク番号
        /// </summary>
        public ushort Bank { get; init; }

        /// <summary>
        /// pbagチャンクのインデックス
        /// </summary>
        public ushort BagIndex { get; init; }

        /// <summary>
        /// 将来的な拡張のために用意された変数。未使用で値は常に「0」。
        /// </summary>
        readonly uint library;

        /// <summary>
        /// 将来的な拡張のために用意された変数。未使用で値は常に「0」。
        /// </summary>
        readonly uint genre;

        /// <summary>
        /// 将来的な拡張のために用意された変数。未使用で値は常に「0」。
        /// </summary>
        readonly uint morph;

        public static int Size => 38;

        public PresetHeader(BinaryReader reader)
        {
            Name = Encoding.ASCII.GetString(reader.ReadBytes(20)).TrimEnd('\0');
            PresetNo = reader.ReadUInt16();
            Bank = reader.ReadUInt16();
            BagIndex = reader.ReadUInt16();
            library = reader.ReadUInt32();
            genre = reader.ReadUInt32();
            morph = reader.ReadUInt32();
        }
    }
}
