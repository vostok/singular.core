using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface ISettingsCache<TSettings>
    {
        //CR: (deniaa) � C# ������� �������� ������, ������������ Task, � ��������� Async!
        Task<List<TSettings>> Get();
    }
}