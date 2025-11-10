using Aro.Common.Domain.Entities;

namespace Aro.Admin.Domain.Entities;

public class SystemSetting : IEntity
{
    public string Key { get; set; }
    public string Value { get; set; }
}
