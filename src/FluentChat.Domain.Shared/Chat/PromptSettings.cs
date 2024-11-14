namespace FluentChat.Chat;

public class PromptSettings
{
    /// <summary>
    /// 随机性：值越大，回复越随机
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// 核采样：与随机性类似，但不要和随机性一起更改
    /// </summary>
    public double? TopP { get; set; }

    /// <summary>
    /// 话题新鲜度：值越大，越有可能扩展到新话题
    /// </summary>
    public double? PresencePenalty { get; set; }

    /// <summary>
    /// 频率惩罚度：值越大，越有可能降低重复字词
    /// </summary>
    public double? FrequencyPenalty { get; set; }
}
