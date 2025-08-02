namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// pmod,imodチャンク内のジェネレーター用のクラス
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2390
    /// </summary>
    internal class Gen
    {
        /// <summary>
        /// 変更するパラメータ(音量やフィルタのエンベロープ等)の番号(ジェネレータID)
        /// </summary>
        public ushort Oper { get; init; }

        /// <summary>
        /// パラメータを変更する量
        /// </summary>
        public ushort Amount { get; init; }

        public static int Size => 4;

        public Gen(BinaryReader reader)
        {
            Oper = reader.ReadUInt16();
            Amount = reader.ReadUInt16();
        }
    }
}
