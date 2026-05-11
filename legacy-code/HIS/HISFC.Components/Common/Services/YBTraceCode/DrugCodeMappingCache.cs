using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Components.Common.Services.YBTraceCode
{
    public static class DrugCodeMappingCache
    {
        private static readonly object SyncRoot = new object();
        private static Dictionary<string, HashSet<string>> _cache;
        private static DateTime _lastLoadUtc = DateTime.MinValue;
        private static TimeSpan _ttl = TimeSpan.FromMinutes(30);

        public static void SetTtlMinutes(int minutes)
        {
            if (minutes <= 0) minutes = 1;
            _ttl = TimeSpan.FromMinutes(minutes);
        }

        public static Dictionary<string, HashSet<string>> GetOrLoad(Func<Dictionary<string, HashSet<string>>> loader)
        {
            // 命中缓存且未过期
            var snapshot = _cache;
            if (snapshot != null && (DateTime.UtcNow - _lastLoadUtc) < _ttl)
                return snapshot;

            lock (SyncRoot)
            {
                if (_cache != null && (DateTime.UtcNow - _lastLoadUtc) < _ttl)
                    return _cache;

                try
                {
                    var data = loader != null ? loader() : null;

                    // 若加载失败，优先返回旧缓存；没有旧缓存则给空字典，避免空引用
                    if (data == null)
                        return _cache ?? new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

                    _cache = data;
                    _lastLoadUtc = DateTime.UtcNow;
                    return _cache;
                }
                catch
                {
                    // 异常时保留旧缓存
                    return _cache ?? new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        public static bool ForceReload(Func<Dictionary<string, HashSet<string>>> loader)
        {
            lock (SyncRoot)
            {
                try
                {
                    var data = loader != null ? loader() : null;
                    if (data == null) return false;

                    _cache = data;
                    _lastLoadUtc = DateTime.UtcNow;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void Clear()
        {
            lock (SyncRoot)
            {
                _cache = null;
                _lastLoadUtc = DateTime.MinValue;
            }
        }
    }
}
