using B14.Model;
using System.Globalization;

namespace B14
{
    public class ProgramServis
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _filesFolder;
        private readonly string _imagesFolder;

        public ProgramServis(IWebHostEnvironment env)
        {
            _env = env;
            _filesFolder = Path.Combine(_env.WebRootPath, "files");
            _imagesFolder = Path.Combine(_env.WebRootPath, "images");
        }

        public List<string> UzmiDatume()
        {
            if (!Directory.Exists(_filesFolder))
            {
                throw new DirectoryNotFoundException($"Directory {_filesFolder} not found");
            }

            return Directory.GetFiles(_filesFolder, "*.txt")
                            .Select(Path.GetFileNameWithoutExtension)
                            .ToList();
        }

        public List<Emisija> GetEmisije()
        {
            var emisije = new List<Emisija>();

            foreach (var fileName in UzmiDatume())
            {
                string filePath = Path.Combine(_filesFolder, $"{fileName}.txt");

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File {filePath} not found");
                }

                if (DateTime.TryParseExact(fileName, "dd.MM.yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datum))
                {
                    var lines = File.ReadAllLines(filePath).ToList();

                    foreach (var line in lines)
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 4)
                        {
                            string tip = parts[2];
                            string slikaPath = Path.Combine(_imagesFolder, $"{tip}.png");
                            emisije.Add(new Emisija
                            {
                                Datum = datum,
                                Vreme = parts[0],
                                Naziv = parts[1],
                                Tip = tip,
                                Slika = slikaPath
                            });
                        }
                    }
                }
                else
                {
                    throw new FormatException($"Filename {fileName} is not in the expected date format 'dd.MM.yy'.");
                }
            }

            return emisije;
        }
    }
}