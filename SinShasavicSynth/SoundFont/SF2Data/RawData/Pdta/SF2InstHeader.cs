using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// sf2のinstチャンク内のインストルメントヘッダー用のレコード
    /// 詳細:https://www.utsbox.com/?p=2090#inst%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal record SF2InstHeader
    {
        /// <summary>
        /// インストルメントの名前。終端文字(\0)含め20文字の文字列。
        /// </summary>
        string name;

        /// <summary>
        /// ibagチャンクのインデックス
        /// </summary>
        ushort bagIndex;

        public static int Size => 22;

        public SF2InstHeader(BinaryReader reader)
        {
            name = Encoding.ASCII.GetString(reader.ReadBytes(20)).TrimEnd('\0');
            bagIndex = reader.ReadUInt16();
        }
    }
}
