using Microsoft.Playwright;

namespace PracticeAuto.Pages
{
    public class DashboardPage
    {
        private readonly IPage _page;
        public DashboardPage(IPage page) => _page = page;

        // No hace await: mejor sincrónico
        public bool IsAt()
        {
            return _page.Url.Contains("/dashboard");
        }
    }
}