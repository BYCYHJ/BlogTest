using FileService.Domain.Entities;

namespace FileService.Domain
{
    public interface IFileRepository
    {
        /// <summary>
        /// 根据hash查找文件是否重复
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<ServerFile?> FindAsync(string hash);

        /// <summary>
        /// 根据id查找文件
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Task<ServerFile?> FindAsync(Guid guid);

        /// <summary>
        /// 上传文件记录到数据库中
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task UploadAsync(ServerFile file);
    }
}