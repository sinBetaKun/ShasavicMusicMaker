using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.Audio;
using SinShasavicSynthSF2.ShasavicObject;
using SinShasavicSynthSF2.SoundFont;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    internal class Voice
    {
        // 基本情報
        ShasavicTone noteTone;
        int vel;
        int ch;
        float freq;

        // 状態管理
        bool isActive;              // このVoiceが現在有効か
        bool isReleased;            // ノートオフ済みか
        double startTime;           // 発音開始時刻（DSP時間など）

        // サンプル再生用
        Sample sample;              // 紐づくSF2の波形（PCMデータ）
        int samplePosition;         // 再生位置（整数部）
        float sampleFrac;           // 再生位置の小数部（補間のため）
        float playbackRate;         // ピッチ変化に応じた再生速度

        // 音量変化（ADSR）
        EnvelopeGenerator ampEnv;   // 音量のエンベロープ
        float currentAmplitude;     // 現在の音量係数

        // モジュレーション
        LFO pitchLFO;               // ピッチを揺らす（ビブラート）
        LFO ampLFO;                 // 音量を揺らす（トレモロ）

        // パン、フィルター、CCなど
        float pan;                  // -1.0（左）～1.0（右）
        Filter filter;              // ローパス等
        Dictionary<int, float> controllers; // CC値のキャッシュ（例：CC74 = カットオフ）

        // 出力先
        AudioBuffer output;         // このVoiceが書き込む出力先バッファ（L/R）
    }
}
