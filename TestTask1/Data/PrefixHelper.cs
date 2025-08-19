namespace TestTask1.Data
{
    /// <summary>
    /// Представляет статический класс-помощник для работы с местоположением/префиксом.
    /// </summary>
    public static class PrefixHelper
    {
        /// <summary>
        /// Вырезает из локации первый префикс и возвращает его. 
        /// Метод не гарантирует, что возвращаемый префикс будет иметь первый символ <paramref name="prefixSymbol"/>.
        /// </summary>
        /// <param name="location">Оставшиеся локация, с которого вырезали префикс.</param>
        /// <param name="prefixSymbol">Символ начала префикса.</param>
        /// <returns>Первый префикс.</returns>
        public static string CutAndGetFirstPrefix(ref string location, char prefixSymbol = '/')
        {
            if (string.IsNullOrEmpty(location))
                return location;

            var index = 0;

            var prefix = "";

            while (index < location.Length)
            {
                var symbol = location[index];

                if (symbol == prefixSymbol && index > 0)
                    break;

                prefix += symbol;

                index++;
            }

            location = location.Substring(index, location.Length - index);

            return prefix;
        }

        /// <summary>
        /// Выполняет проверку указанной локации на соответствие формату <c>"/path1.../pathN"</c>.
        /// </summary>
        /// <param name="location">Локация, которую надо проверить.</param>
        /// <param name="prefixSymbol">Символ начала префикса.</param>
        /// <returns>Возвращает <c>true</c>, если указанная локация соответствует формату. Иначе <c>false</c>.</returns>
        public static bool IsValidatePath(string location, char prefixSymbol = '/')
        {
            if (string.IsNullOrEmpty(location)) // Пустая строка это не путь.
                return false;

            var prefix = CutAndGetFirstPrefix(ref location);

            if (prefix.Length <= 2 || prefix[0] != prefixSymbol)
                return false;

            if (string.IsNullOrEmpty(location)) // Если после обрезки путь стал пустым, то вернем истину, ведь путь просто кончился
                return true;

            return PrefixHelper.IsValidatePath(location);
        }
    }
}
