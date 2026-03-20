using UnityEngine;

namespace SongShuVegetable.Utils
{
    /// <summary>
    /// 集中式日志工具。
    /// 生产包自动静默（UNITY_EDITOR 或 DEVELOPMENT_BUILD 下才输出），
    /// 也可在运行时通过 Log.Enabled 手动开关。
    /// 用法：Log.Info("msg") / Log.Info("[Plant] msg", context)
    /// </summary>
    public static class Log
    {
        public static bool Enabled = true;

        // ── 普通信息 ──────────────────────────────────────────
        public static void Info(string message, Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!Enabled) return;
            if (context != null)
                Debug.Log(message, context);
            else
                Debug.Log(message);
#endif
        }

        // ── 警告 ──────────────────────────────────────────────
        public static void Warning(string message, Object context = null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!Enabled) return;
            if (context != null)
                Debug.LogWarning(message, context);
            else
                Debug.LogWarning(message);
#endif
        }

        // ── 错误（始终输出，不受 Enabled 控制）───────────────
        public static void Error(string message, Object context = null)
        {
            if (context != null)
                Debug.LogError(message, context);
            else
                Debug.LogError(message);
        }

        // ── 带 Tag 的快捷方法 ──────────────────────────────────
        public static void Info(string tag, string message, Object context = null)
            => Info($"[{tag}] {message}", context);

        public static void Warning(string tag, string message, Object context = null)
            => Warning($"[{tag}] {message}", context);

        public static void Error(string tag, string message, Object context = null)
            => Error($"[{tag}] {message}", context);
    }
}
