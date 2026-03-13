namespace KszUtil.MyStorage
{
    /// <summary>
    /// データ名ドメインクラス
    /// </summary>
    public class DatName
    {
        public DatName(string name, string extension = "dat")
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                Name = name;
            }
            else
            {
                Name = $"{name}.{extension}";
            }
        }

        public string Name { get; }
        public string FlattenName => Flatten(Name);

        //パスを平坦化
        public static string Flatten(string path)
        {
            return path.Replace("/", "__").Replace(":", "__").Replace("\\", "__").Replace("=", "__").Replace("&", "__").Replace("?", "__");
        }

        public static DatName ToDatName(string datName) => new DatName(datName);
    }
}