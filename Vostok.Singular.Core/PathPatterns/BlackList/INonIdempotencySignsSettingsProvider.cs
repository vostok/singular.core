using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal interface INonIdempotencySignsSettingsProvider
    {
        //CR: (deniaa) � C# ������� �������� ������, ������������ Task, � ��������� Async!
        Task<NonIdempotencySignsSettings> Get();
    }
}