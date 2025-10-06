using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShasavicMusicMaker.DimensionData
{
    internal static class DimensionInfo
    {
        /// <summary>
        /// 現バージョンで対応している次元の数。
        /// </summary>
        public static int MaxDimension => 6;

        /// <summary>
        /// 第N成分に(N+1)次元の倍数が収められている
        /// </summary>
        public static ImmutableArray<float> Coefs { get; private set; } = [
            2f,
            3 / 2f,
            5 / 4f,
            7 / 4f,
            11 / 4f,
            13 / 4f
            ];

        /// <summary>
        /// 各次元の色が収められている。
        /// </summary>
        public static ImmutableArray<Brush> Colors { get; private set; } = [
            new SolidColorBrush(Color.FromRgb(0xA9, 0xA9, 0xA9)),
            new SolidColorBrush(Color.FromRgb(0xF2, 0x79, 0x92)),
            new SolidColorBrush(Color.FromRgb(0x6C, 0xD9, 0x85)),
            new SolidColorBrush(Color.FromRgb(0xB5, 0x98, 0xEE)),
            new SolidColorBrush(Color.FromRgb(0xFF, 0xC2, 0x47)),
            new SolidColorBrush(Color.FromRgb(0xB5, 0xB5, 0x00)),
            ];

        /// <summary>
        /// 和音図を作るときに、重複を避けるために水平方向に表示をずらす必要がある次元線をtrueで示している。
        /// </summary>
        public static ImmutableArray<bool> avoidDupl { get; private set; } = [
            true,
            true,
            true,
            false,
            false,
            true,
            ];

        /// <summary>
        /// 和音図を作るときに次元線をどの順序で並べるか。
        /// 0は1次元を表している。
        /// </summary>
        public static ImmutableArray<int> DimensionLineOrder { get; private set; } = [
            5,
            1,
            3,
            4,
            2,
            0,
            ];

        public static int[] MakeFomulaArray() => new int[MaxDimension];
    }
}
