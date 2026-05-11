using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neusoft.HISFC.Models.SqlSugar;

namespace Neusoft.HISFC.Components.Common.Services.YBTraceCode
{
    public static class DrugBaseInfoCache
    {
        private static readonly object SyncRoot = new object();
        private static Dictionary<string, PhaComBaseinfo> _cache;
        private static DateTime _lastLoadUtc = DateTime.MinValue;
        private static TimeSpan _ttl = TimeSpan.FromMinutes(30);

        public static void SetTtlMinutes(int minutes)
        {
            if (minutes <= 0) minutes = 1;
            _ttl = TimeSpan.FromMinutes(minutes);
        }

        public static Dictionary<string, PhaComBaseinfo> GetOrLoad(Func<Dictionary<string, PhaComBaseinfo>> loader)
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
                        return _cache ?? new Dictionary<string, PhaComBaseinfo>(StringComparer.OrdinalIgnoreCase);

                    _cache = data;
                    _lastLoadUtc = DateTime.UtcNow;
                    return _cache;
                }
                catch
                {
                    // 异常时保留旧缓存
                    return _cache ?? new Dictionary<string, PhaComBaseinfo>(StringComparer.OrdinalIgnoreCase);
                }
            }
        }

        public static bool ForceReload(Func<Dictionary<string, PhaComBaseinfo>> loader)
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

        // 便捷方法：根据 drug_code 获取药品信息
        public static PhaComBaseinfo GetDrugInfo(string drugCode)
        {
            if (string.IsNullOrEmpty(drugCode)) return null;

            var cache = _cache;
            if (cache == null) return null;

            PhaComBaseinfo drugInfo;
            cache.TryGetValue(drugCode, out drugInfo);
            return drugInfo;
        }
    }
}
