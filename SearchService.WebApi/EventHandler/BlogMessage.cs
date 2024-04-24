namespace SearchService.WebApi.EventHandler
{
    /// <summary>
    /// 消息队列中的Blog信息
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="userId"></param>
    /// <param name="previewPhoto"></param>
    public record BlogMessage(string id,string blogTitle,string blogContent,string userId,string previewPhoto);
}
