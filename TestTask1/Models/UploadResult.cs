namespace TestTask1.Models
{
    /// <summary>
    /// Представляет класс результатов загрузки файла.
    /// </summary>
    public class UploadResult
    {
        /// <summary>
        /// Получает или задает список ошибок.
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
    }
}
