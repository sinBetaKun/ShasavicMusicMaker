namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// pmod,imodチャンク内のモジュレータ用レコード
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#pmod%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal record SF2Mod
    {
        /// <summary>
        /// モジュレータの元となる入力ソース。
        /// フラグ形式でMIDI CCやピッチベンドなどを指定する。
        /// </summary>
        ushort srcOper;

        /// <summary>
        /// 入力ソースで操作するジェネレータID
        /// </summary>
        ushort destOper;

        /// <summary>
        /// 操作するジェネレータ量
        /// </summary>
        short modAmount;

        /// <summary>
        /// モジュレータの元となる入力ソース。
        /// フラグ形式でMIDI CCやピッチベンドなどを指定する。
        /// </summary>
        ushort amtSrcOper;

        /// <summary>
        /// 変化量は線形か？曲線か？
        /// </summary>
        ushort modTransOper;

        public static int Size => 10;

        public SF2Mod(BinaryReader reader)
        {
            srcOper = reader.ReadUInt16();
            destOper = reader.ReadUInt16();
            modAmount = reader.ReadInt16();
            amtSrcOper = reader.ReadUInt16();
            modTransOper = reader.ReadUInt16();
        }
    }
}
