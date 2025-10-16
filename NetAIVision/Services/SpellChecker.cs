using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAIVision.Services
{
    public static class SpellChecker
    {
        private static Hunspell _hunspell;

        // 初始化拼寫檢查器（只需調用一次）
        public static void Initialize(string affPath, string dicPath)
        {
            _hunspell = new Hunspell(affPath, dicPath);
        }

        // 檢查單詞是否拼寫正確
        public static bool Check(string word)
        {
            if (_hunspell == null)
                throw new InvalidOperationException("請先呼叫 Initialize 初始化拼寫檢查器。");

            if (string.IsNullOrWhiteSpace(word))
                return false;

            return _hunspell.Spell(word.Trim());
        }

        // 提供拼寫建議
        public static List<string> Suggest(string word)
        {
            if (_hunspell == null)
                throw new InvalidOperationException("請先呼叫 Initialize 初始化拼寫檢查器。");

            if (string.IsNullOrWhiteSpace(word))
                return new List<string>();

            return _hunspell.Suggest(word.Trim()) ?? new List<string>();
        }

        // 釋放資源
        public static void Dispose()
        {
            _hunspell?.Dispose();
            _hunspell = null;
        }
    }
}