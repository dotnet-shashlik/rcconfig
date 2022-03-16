using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.AutoMapper;
using Shashlik.Kernel.Dependency;
using Shashlik.RC.Server.Common;
using Shashlik.RC.Server.Data;
using Shashlik.RC.Server.Data.Entities;
using Shashlik.RC.Server.Services.Log.Dtos;
using Shashlik.RC.Server.Services.Log.Inputs;
using Shashlik.Utils.Extensions;

namespace Shashlik.RC.Server.Services.Log
{
    [Scoped]
    public class LogService
    {
        public LogService(RCDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private RCDbContext DbContext { get; }

        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="userId">操作人id</param>
        /// <param name="userName">操作人username</param>
        /// <param name="logType">日志类型</param>
        /// <param name="fileId">文件id</param>
        /// <param name="fileName">文件名</param>
        /// <param name="resourceId">资源id</param>
        /// <param name="beforeContent">操作前内容</param>
        /// <param name="afterContent">操作后内容</param>
        /// <param name="logTime">记录时间</param>
        /// <param name="autoSave"></param>
        /// <returns></returns>
        public async Task Add(
            int userId,
            string userName,
            LogType logType,
            int fileId,
            string fileName,
            string resourceId,
            string beforeContent,
            string afterContent,
            long? logTime = null,
            bool autoSave = false
        )
        {
            var log = new Logs
            {
                LogTime = logTime ?? DateTime.Now.GetLongDate(),
                LogType = logType.ToString(),
                UserId = userId,
                UserName = userName,
                FileId = fileId,
                FileName = fileName,
                ResourceId = resourceId,
                BeforeContent = beforeContent,
                AfterContent = afterContent
            };

            await DbContext.AddAsync(log);
            if (autoSave)
                await DbContext.SaveChangesAsync();
        }

        public async Task<PageModel<LogListDto>> List(string resourceId, SearchLogInput input)
        {
            return await DbContext.Logs
                .Where(r => r.ResourceId == resourceId)
                .WhereIf(!input.FileName.IsNullOrWhiteSpace(), r => r.FileName.StartsWith(input.FileName!))
                .WhereIf(input.FileId.HasValue, r => r.FileId == input.FileId)
                .OrderByDescending(r => r.Id)
                .QueryTo<LogListDto>()
                .PageQuery(input);
        }

        public async Task<LogDetailDto> Get(string resourceId, int logId)
        {
            return await DbContext.Logs
                .Where(r => r.Id == logId && r.ResourceId == resourceId)
                .QueryTo<LogDetailDto>()
                .FirstOrDefaultAsync();
        }
    }
}