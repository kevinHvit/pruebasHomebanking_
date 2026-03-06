using NUnit.Framework;
using Allure.NUnit;                
using Allure.NUnit.Attributes;     
using PracticeAuto.Pages;
using PracticeAuto.Support;
using static Microsoft.Playwright.Assertions;

namespace PracticeAuto.Tests
{
    [TestFixture]
    
    [AllureSuite("Login Suite")]
    [AllureFeature("Login Correcto - Logout")]
    [AllureOwner("Kevin Castrillo")]
    public class LoginTests : BaseTest
    {
        private LoginPage login = null!;

        [SetUp]
        public async Task SetupTestPage()
        {
            login = new LoginPage(Page);
            await login.GoToAsync();
        }

        [AllureFeature("Login Correcto - Logout")]
        [Test]
        public async Task LoginCorrecto_Y_Logout()
        {
            await login.LoginAsync("demo", "demo123");
            var logoutBtn = Page.Locator("#logout-btn");
            await Expect(logoutBtn).ToBeVisibleAsync(new() { Timeout = 10_000 });
            await logoutBtn.ClickAsync();
        }

        [AllureFeature("Login Incorrecto")]
        [Test]
        public async Task LoginIncorrecto()
        {
            await login.LoginAsync("wrong", "wrong");
            var msg = login.ErrorMessage();
            await Expect(msg).ToBeVisibleAsync(new() { Timeout = 10_000 });
            await Expect(msg).ToContainTextAsync("Usuario");
        }

        [AllureFeature("Bloqueo 3 Intentos")]
        [Test]
        public async Task BloqueoDespuesDe3Intentos()
        {
            for (int i = 0; i < 3; i++)
                await login.LoginAsync("locked", "locked");

            var msg = login.ErrorMessage();
            await Expect(msg).ToBeVisibleAsync(new() { Timeout = 10_000 });
            await Expect(msg).ToContainTextAsync("bloqueada");
        }
    }
}
