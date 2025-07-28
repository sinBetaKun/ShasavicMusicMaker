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
        ushort genOper;

        /// <summary>
        /// パラメータを変更する量
        /// </summary>
        ushort genAmount;

        public static int Size => 4;

        public SF2Gen(BinaryReader reader)
        {
            genOper = reader.ReadUInt16();
            genAmount = reader.ReadUInt16();
        }
    }
}
