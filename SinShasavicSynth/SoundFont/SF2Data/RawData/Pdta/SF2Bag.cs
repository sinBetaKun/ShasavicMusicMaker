namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// sf2のpbag,ibagチャンク内のバッグ用レコード
    /// 詳細:https://www.utsbox.com/?p=2090#pbag%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal record SF2Bag
    {
        /// <summary>
        /// pgenチャンクのインデックス
        /// </summary>
        public ushort GenIndex;

        /// <summary>
        /// pmodチャンクのインデックス
        /// </summary>
        ushort modIndex;

        public static int Size => 4;

        public SF2Bag(BinaryReader reader)
        {
            GenIndex = reader.ReadUInt16();
            modIndex = reader.ReadUInt16();
        }
    }
}
