namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// pmod,imodチャンク内のジェネレーター用のレコード
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2390
    /// </summary>
    internal record SF2Gen
    {
        /// <summary>
        /// 変更するパラメータ(音量やフィルタのエンベロープ等)の番号(ジェネレータID)
        /// </summary>
        public readonly ushort Oper;

        /// <summary>
        /// パラメータを変更する量
        /// </summary>
        public readonly ushort Amount;

        public static int Size => 4;

        public SF2Gen(BinaryReader reader)
        {
            Oper = reader.ReadUInt16();
            Amount = reader.ReadUInt16();
        }
    }
}
