using B14.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace B14.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ProgramServis _programServis;

        public IndexModel(ProgramServis programServis)
        {
            _programServis = programServis;
        }

        public List<SelectListItem> Datumi { get; set; }
        public List<SelectListItem> Emisije { get; set; }
        public string SelektovanDatum { get; set; }
        public string SelektovanTipEmisije { get; set; }
        public List<Emisija> FiltriraneEmisije { get; set; }

        public void OnGet()
        {
            Datumi = _programServis.GetEmisije()
                                .Select(e => e.Datum)
                                .Distinct()
                                .OrderBy(d => d)
                                .Select(d => new SelectListItem { Value = d.ToString("yyyy-MM-dd"), Text = d.ToString("yyyy-MM-dd") })
                                .ToList();

            if (Datumi.Count > 0)
            {
                var prviDatum = DateTime.Parse(Datumi.First().Value);
                Emisije = _programServis.GetEmisije()
                                .Where(e => e.Datum == prviDatum)
                                .Select(e => e.Tip)
                                .Distinct()
                                .Select(t => new SelectListItem { Value = t, Text = t })
                                .ToList();
            }
            else
            {
                Emisije = new List<SelectListItem>();
            }
            FiltriraneEmisije = new List<Emisija>();
        }

        public void OnPost(string selectedDate, string selectedShow)
        {
            SelektovanDatum = selectedDate;
            SelektovanTipEmisije = selectedShow;

            Datumi = _programServis.GetEmisije()
                                .Select(e => e.Datum)
                                .Distinct()
                                .OrderBy(d => d)
                                .Select(d => new SelectListItem { Value = d.ToString("yyyy-MM-dd"), Text = d.ToString("yyyy-MM-dd") })
                                .ToList();

            if (DateTime.TryParse(selectedDate, out DateTime parsedDate))
            {
                var emisije = _programServis.GetEmisije()
                                          .Where(e => e.Datum == parsedDate)
                                          .ToList();

                Emisije = emisije.Select(e => e.Tip)
                               .Distinct()
                               .Select(t => new SelectListItem { Value = t, Text = t })
                               .ToList();

                if (!string.IsNullOrEmpty(selectedShow))
                {
                    FiltriraneEmisije = emisije.Where(e => e.Tip == selectedShow).ToList();
                }
                else
                {
                    FiltriraneEmisije = emisije;
                }
            }
            else
            {
                Emisije = new List<SelectListItem>();
                FiltriraneEmisije = new List<Emisija>();
            }
        }
    }
}
