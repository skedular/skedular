using System.Diagnostics;

namespace Enterprise.Shared.Metrics;

public interface ITaggable<in T>
{
    TagList GetTags(T source);
}
