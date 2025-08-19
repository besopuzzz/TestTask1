using Microsoft.AspNetCore.Mvc;
using TestTask1.Data;
using TestTask1.Models;

namespace TestTask1.Controllers
{
    /// <summary>
    /// ������������ API ��� ������ � ���������� ����������.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly Platform root;

        /// <summary>
        /// �������������� ��������� ������ <see cref="PlatformsController"/>.
        /// </summary>
        /// <param name="root">��������� ������� �� ������ � �����������.</param>
        public PlatformsController(Platform root)
        {
            this.root = root;
        }

        /// <summary>
        /// �������� ������ �������� �� ��������� �������.
        /// </summary>
        /// <param name="path">������� � ������� '/path1/.../pathN'.</param>
        /// <returns>���������� ��������� ������� <see cref="IActionResult"/>, � ��� �� ������ �������� <see cref="IEnumerable{Platform}"/>.</returns>
        /// <response code="200">�������� ����������� ��������.</response>
        /// <response code="400">�������� ������ �������.</response>
        /// <response code="404">��� ���������� ��� ������ ���������.</response>
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
        /// ��������� ������ � ���������� �� ���������������� .txt �����.
        /// ����� ������� ��������� ���� ������� ������ �� ����� ����������� ������.
        /// </summary>
        /// <param name="file">���� � �������.</param>
        /// <returns>���������� ��������� ������� <see cref="IActionResult"/>, � ��� �� ��������� ��������� ����� <see cref="UploadResult"/>.</returns>
        /// <response code="200">�������� �������� � ��������� �����.</response>
        /// <response code="400">�������� ������ �����.</response>
        [HttpPost(Name = "Upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileModel file)
        {
            var result = new UploadResult();

            var content = "";

            try
            {
                if (file.File == null)
                    throw new Exception("�� ������ ����.");

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

                errors.Add("�� ����� �������� ����� ��������� ������.");

                errors.Add(ex.Message);

                root.Clear();

                return BadRequest(result);
            }
        }
    }
}
