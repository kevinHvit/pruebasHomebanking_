using Microsoft.Playwright;

namespace PracticeAuto.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        private ILocator Username => _page.Locator("#username");
        private ILocator Password => _page.Locator("#password");
        private ILocator LoginBtn => _page.Locator("#login-btn");
        private ILocator ErrorMsg => _page.Locator("#login-error");

        public LoginPage(IPage page) => _page = page;

        public async Task GoToAsync(string url = "https://homebanking-demo-tests.netlify.app/")
            => await _page.GotoAsync(url);

        public async Task LoginAsync(string username, string password)
        {
            await Username.FillAsync("");
            await Password.FillAsync("");
            await Username.FillAsync(username);
            await Password.FillAsync(password);
            await LoginBtn.ClickAsync();
        }

        public ILocator ErrorMessage() => ErrorMsg;
    }
}