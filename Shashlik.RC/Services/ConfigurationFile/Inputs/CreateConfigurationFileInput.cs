using System.ComponentModel.DataAnnotations;

#nullable disable
namespace Shashlik.RC.Services.ConfigurationFile.Inputs
{
    public class CreateConfigurationFileInput
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 文件类型,yaml/json
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 文件内容
        /// </summary>
        public string Content { get; set; }
    }
}