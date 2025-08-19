namespace TestTask1.Data
{
    /// <summary>
    /// Представляет класс ошибок, когда местоположение/префикс имеет неверный формат.
    /// </summary>
    public class PrefixFormatException : Exception
    {
        /// <summary>
        /// Инициализирует экземпляр класса <see cref="PrefixFormatException"/>.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        public PrefixFormatException(string? message) : base(message)
        {
        }

        /// <summary>
        /// Вызывает исключение, если <paramref name="prefix"/> имеет неверный формат.
        /// </summary>
        /// <param name="prefix">Местоположение/префикс, который необходимо проверить.</param>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <exception cref="PrefixFormatException"></exception>
        public static void ThrowIfNotValid(string prefix, string? message = null)
        {
            if (!PrefixHelper.IsValidatePath(prefix))
            {
                if (message == null)
                    message = $"Указанный путь/префикс '{prefix}' имеет неверный формат.";

                throw new PrefixFormatException(message);
            }
        }
    }
}
