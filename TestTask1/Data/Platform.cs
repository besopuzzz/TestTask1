namespace TestTask1.Data
{
    /// <summary>
    /// Представляет класс рекламных площадок, которые имеет вложении площадки в виде иерархии.
    /// </summary>
    public sealed class Platform
    {
        /// <summary>
        /// Получает наименование платформы, расположенную на этой <see cref="Location"/> локации.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Получает префикс локации относительно родительской платформы.
        /// </summary>
        public string Prefix => prefix;

        /// <summary>
        /// Получает локацию платформы.
        /// </summary>
        public string Location
        {
            get
            {
                return parent == null ? Prefix : parent.Location + Prefix;
            }
        }

        /// <summary>
        /// Возвращает первую найденную вложенную платформу.
        /// </summary>
        /// <param name="prefix">Префикс вложенной платформы.</param>
        /// <returns></returns>
        /// <exception cref="PrefixFormatException"></exception>
        public Platform? this[string prefix]
        {
            get
            {
                PrefixFormatException.ThrowIfNotValid(prefix);

                childs.TryGetValue(prefix, out var platform);

                return platform;
            }
        }

        /// <summary>
        /// Возвращает коллекцию всех вложенных платформ.
        /// </summary>
        public IReadOnlyCollection<Platform> Platforms => childs.Values;

        private readonly Dictionary<string, Platform> childs;
        private string prefix;
        private Platform? parent;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Data.Platform"/>.
        /// </summary>
        internal Platform()
        {
            Name = "";
            childs = new Dictionary<string, Platform>();
            prefix = string.Empty;
        }

        /// <summary>
        /// <inheritdoc cref="Data.Platform"/>
        /// </summary>
        /// <param name="prefix">Префикс локации.</param>
        /// <exception cref="PrefixFormatException"></exception>
        public Platform(string prefix) : this()
        {
            PrefixFormatException.ThrowIfNotValid(prefix);

            this.prefix = prefix;
        }

        /// <summary>
        /// Добавляет вложенную платформу.
        /// </summary>
        /// <param name="child">Экземпляр вложенной платформы.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(Platform child)
        {
            ArgumentNullException.ThrowIfNull(child);

            var location = this[child.Prefix];

            if (location != null)
                throw new ArgumentException($"Платформа с префиксом {prefix} уже существует у платформы {location.Name}.");

            if (location == child)
                return;

            InitializeAndAdd(child);
        }

        private void InitializeAndAdd(Platform child)
        {
            child.parent?.Remove(child);

            child.parent = this;

            childs.Add(child.prefix, child);
        }

        /// <summary>
        /// Получает вложенную платформу или создает ее.
        /// </summary>
        /// <param name="prefix">Префикс локации.</param>
        /// <returns>Найденная или созданная вложенная платформа.</returns>
        /// <exception cref="PrefixFormatException"></exception>
        public Platform GetOrAdd(string prefix)
        {
            var location = this[prefix];

            if (location == null)
            {
                location = new Platform();

                location.prefix = prefix;

                InitializeAndAdd(location);
            }

            return location;
        }

        /// <summary>
        /// Удаляет вложенную платформу.
        /// </summary>
        /// <param name="child">Экземпляр вложенной платформы.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Remove(Platform child)
        {
            ArgumentNullException.ThrowIfNull(child);

            childs.Remove(child.prefix);
        }

        /// <summary>
        /// Удаляет вложенную платформу, найденную по префиксу.
        /// </summary>
        /// <param name="prefix">Префикс, по которому ищет платформы.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Remove(string prefix)
        {
            Remove(this[prefix]!);
        }

        /// <summary>
        /// Удаляет все вложенные платформы.
        /// </summary>
        public void Clear()
        {
            childs.Clear();
        }

        /// <summary>
        /// Выполняет поиск платформ с учетом вложенных локаций. 
        /// </summary>
        /// <param name="location">Локация искомых площадок.</param>
        /// <returns>Коллекция найденных платформ <see cref="IEnumerable{T}"/>.</returns>
        public IEnumerable<Platform> FindPlatforms(string location)
        {
            PrefixFormatException.ThrowIfNotValid(location);

            List<Platform> platforms = new List<Platform>();

            var prefix = PrefixHelper.CutAndGetFirstPrefix(ref location);

            if(childs.TryGetValue(prefix, out var location2))
            {
                if (!string.IsNullOrEmpty(location2.Name))
                    platforms.Add(location2);

                if (!string.IsNullOrEmpty(location))
                    platforms.AddRange(location2.FindPlatforms(location));
            }

            return platforms;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Location;
        }

        /// <summary>
        /// Разбирает вводные данные и выстраивает иерархию.
        /// </summary>
        /// <param name="txt">Данные .txt файла.</param>
        public void Parse(string txt)
        {
            Clear();

            var lines = txt.Split(Environment.NewLine);

            if (lines.Length == 0)
                throw new Exception($"В вводных данных отсутствуют данные.");

            for (int i = 0; i < lines.Length; i++)
            {
                var data = lines[i].Split(':');
                var lineNumber = i + 1;

                if (data.Length != 2)
                    throw new Exception($"Строка {lineNumber} не соответствует формату: 'name:/path1/path2'. Возможно, отсутствует разделитель ':'.");

                var platform = data[0];

                var locations = data[1];

                if (string.IsNullOrEmpty(platform) || string.IsNullOrEmpty(locations))
                    throw new Exception($"Строка {lineNumber} не соответствует формату: данные о имени и/или пути отсутствуют.");

                foreach (var location in locations.Split(','))
                {
                    PrefixFormatException.ThrowIfNotValid(location, $"Путь '{location}' в строке {lineNumber} не соответствует формату.");

                    var endPath = location;

                    Parse(this, ref endPath, platform);
                }
            }
        }

        /// <summary>
        /// Разбирает локацию на префиксы и создает вложенные платформы.
        /// В случае, когда в пути есть префиксы неизвестных платформ, для них будет создана платформа без наименования <see cref="Platform.Name"/>.
        /// </summary>
        /// <param name="parent">Экземпляр платформы, для которой создаем вложенные платформы.</param>
        /// <param name="location">Локация, которое необходимо разобрать на префиксы.</param>
        /// <param name="platform">Наименование платформы, для которой разбираем локацию.</param>
        private static void Parse(Platform parent, ref string location, string platform)
        {
            var prefix = PrefixHelper.CutAndGetFirstPrefix(ref location);

            var location2 = parent.GetOrAdd(prefix);

            if (string.IsNullOrEmpty(location))
            {
                location2.Name = platform;
            }
            else Parse(location2, ref location, platform);
        }
    }
}
