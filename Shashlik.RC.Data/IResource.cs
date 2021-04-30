namespace Shashlik.RC.Data
{
    /// <summary>
    /// 资源定义
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// 资源id
        /// </summary>
        string ResourceId { get; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResourceName { get; }
    }
}