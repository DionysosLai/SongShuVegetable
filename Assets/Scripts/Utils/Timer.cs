using System;
using System.Collections;
using UnityEngine;

namespace SongShuVegetable.Utils
{
    /// <summary>
    /// 轻量协程计时工具，挂载到任意 MonoBehaviour 上驱动。
    /// 用法：
    ///   Timer.Delay(this, 2f, () => Log.Info("2秒后执行"));
    ///   Timer.Repeat(this, 1f, () => Log.Info("每秒执行"));
    ///   var handle = Timer.Repeat(this, 0.5f, callback);
    ///   Timer.Cancel(this, handle);
    /// </summary>
    public static class Timer
    {
        // ── 延迟执行一次 ──────────────────────────────────────
        public static Coroutine Delay(MonoBehaviour runner, float seconds, Action onComplete)
        {
            if (runner == null || !runner.gameObject.activeInHierarchy) return null;
            return runner.StartCoroutine(DelayRoutine(seconds, onComplete));
        }

        // ── 每隔 interval 秒重复执行，repeatCount=-1 表示无限 ──
        public static Coroutine Repeat(MonoBehaviour runner, float interval, Action onTick,
            int repeatCount = -1)
        {
            if (runner == null || !runner.gameObject.activeInHierarchy) return null;
            return runner.StartCoroutine(RepeatRoutine(interval, onTick, repeatCount));
        }

        // ── 取消协程 ─────────────────────────────────────────
        public static void Cancel(MonoBehaviour runner, Coroutine coroutine)
        {
            if (runner != null && coroutine != null)
                runner.StopCoroutine(coroutine);
        }

        // ── 下一帧执行 ────────────────────────────────────────
        public static Coroutine NextFrame(MonoBehaviour runner, Action onComplete)
        {
            if (runner == null || !runner.gameObject.activeInHierarchy) return null;
            return runner.StartCoroutine(NextFrameRoutine(onComplete));
        }

        // ── 内部协程 ──────────────────────────────────────────
        private static IEnumerator DelayRoutine(float seconds, Action onComplete)
        {
            yield return new WaitForSeconds(seconds);
            onComplete?.Invoke();
        }

        private static IEnumerator RepeatRoutine(float interval, Action onTick, int repeatCount)
        {
            var wait = new WaitForSeconds(interval);
            int count = 0;
            while (repeatCount < 0 || count < repeatCount)
            {
                yield return wait;
                onTick?.Invoke();
                count++;
            }
        }

        private static IEnumerator NextFrameRoutine(Action onComplete)
        {
            yield return null;
            onComplete?.Invoke();
        }
    }
}
