namespace TestTask1.Models
{
    /// <summary>
    /// Представляет класс загружаемого файла. Этот класс является оберткой класса <see cref="IFormFile"/>, так как Swagger 
    /// отказывается работать напрямую.
    /// </summary>
    public class UploadFileModel
    {
        /// <summary>
        /// Получает или задает файл <see cref="IFormFile"/>.
        /// </summary>
        public IFormFile? File { get; set; }
    }
}
