using Microsoft.AspNetCore.Mvc;
using TestTask1.Data;
using TestTask1.Models;

namespace TestTask1.Controllers
{
    /// <summary>
    /// Представляет API для работы с рекламными площадками.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly Platform root;

        /// <summary>
        /// Инициализирует экземпляр класса <see cref="PlatformsController"/>.
        /// </summary>
        /// <param name="root">Экземпляр сервиса по работе с площадками.</param>
        public PlatformsController(Platform root)
        {
            this.root = root;
        }

        /// <summary>
        /// Получает список площадок по указанной локации.
        /// </summary>
        /// <param name="path">Локация в формате '/path1/.../pathN'.</param>
        /// <returns>Возвращает результат запроса <see cref="IActionResult"/>, а так же список площадок <see cref="IEnumerable{Platform}"/>.</returns>
        /// <response code="200">Успешное возвращение площадок.</response>
        /// <response code="400">Неверный формат локации.</response>
        /// <response code="404">Нет подходящих под запрос вариантов.</response>
        [HttpGet(Name = "Get")]
        public IActionResult Get([FromQuery] string path)
        {
            try
            {
                var result = root.FindPlatforms(path).Select(x => x.Name);
                
                if (result.Any())
                    return Ok(result);
                else return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Errors = ex.Message });
            }
        }

        /// <summary>
        /// Загружает данные о площадок из форматированного .txt файла.
        /// Любая попытка загрузить файл очищает память от ранее загруженных данных.
        /// </summary>
        /// <param name="file">Файл с данными.</param>
        /// <returns>Возвращает результат запроса <see cref="IActionResult"/>, а так же результат обработки файла <see cref="UploadResult"/>.</returns>
        /// <response code="200">Успешная загрузка и обработка файла.</response>
        /// <response code="400">Неверный формат файла.</response>
        [HttpPost(Name = "Upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileModel file)
        {
            var result = new UploadResult();

            var content = "";

            try
            {
                if (file.File == null)
                    throw new Exception("Не указан файл.");

                using (var sr = new StreamReader(file.File.OpenReadStream()))
                {
                    content = await sr.ReadToEndAsync();
                }

                root.Parse(content);

                return Ok(result);
            }
            catch (Exception ex)
            {
                var errors = result.Errors;

                errors.Add("Во время загрузки файла произошла ошибка.");

                errors.Add(ex.Message);

                root.Clear();

                return BadRequest(result);
            }
        }
    }
}
