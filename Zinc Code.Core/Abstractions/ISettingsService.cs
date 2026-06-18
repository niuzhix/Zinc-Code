namespace Zinc_Code.Core.Abstractions;

public interface ISettingsService
{
    /// <summary>
    /// 预加载设置文件
    /// </summary>
    void Preload();

    /// <summary>
    /// 读取指定项
    /// </summary>
    T Read<T>(string key, T defaultValue = default!);

    /// <summary>
    /// 写入指定项（仅内存，不保存到文件）
    /// </summary>
    void Write(string key, object value);

    /// <summary>
    /// 写入指定项并立即保存到文件
    /// </summary>
    void WriteAndSave(string key, object value);

    /// <summary>
    /// 保存所有设置到文件
    /// </summary>
    void Save();
}