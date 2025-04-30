using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutomatedTestsProyecto.Tests
{
    [TestFixture]
    public class TiendaVirtualTests
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }

        [Test]
        public void testNavigation()
        {
            int numberOfUsers = 5;
            var tasks = new List<Task>();

            for (int i = 0; i < numberOfUsers; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var localDriver = new ChromeDriver();
                    localDriver.Navigate().GoToUrl("https://localhost:7195/");
                    localDriver.Quit();
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Test]
        public void testContactForm()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/Product/Contact");
            IWebElement nameField = driver.FindElement(By.Name("Name"));
            IWebElement emailField = driver.FindElement(By.Name("Email"));
            IWebElement messageField = driver.FindElement(By.Name("Message"));
            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            nameField.SendKeys("Test User");
            emailField.SendKeys("test.user@example.com");
            messageField.SendKeys("This is a test message.");
            submitButton.Click();
        }

        [Test]
        public void testProductCreation()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/Product/Create");
            IWebElement nameField = driver.FindElement(By.Name("Name"));
            IWebElement priceField = driver.FindElement(By.Name("Price"));
            IWebElement descriptionField = driver.FindElement(By.Name("Description"));
            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            string productName = "Test Product";
            nameField.SendKeys(productName);
            priceField.SendKeys("100");
            descriptionField.SendKeys("This is a test product.");
            submitButton.Click();

            driver.Navigate().GoToUrl("https://localhost:7195/");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.CssSelector("table.table tbody tr")).Any());

            var productList = driver.FindElements(By.CssSelector("table.table tbody tr"));
            bool productFound = productList.Any(product => product.Text.Contains(productName));

            Assert.That(productFound, Is.True, "Producto no encontrado en la lista.");
        }

        [Test]
        public void emptyContactForm()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/Product/Contact");
            IWebElement nameField = driver.FindElement(By.Name("Name"));
            IWebElement emailField = driver.FindElement(By.Name("Email"));
            IWebElement messageField = driver.FindElement(By.Name("Message"));
            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            nameField.Clear();
            emailField.Clear();
            messageField.Clear();
            submitButton.Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.CssSelector(":invalid")).Any());

            bool isNameFieldInvalid = nameField.GetAttribute("validationMessage") != null;
            bool isEmailFieldInvalid = emailField.GetAttribute("validationMessage") != null;
            bool isMessageFieldInvalid = messageField.GetAttribute("validationMessage") != null;

            Assert.That(isNameFieldInvalid, Is.True, "El campo de nombre no es inválido.");
            Assert.That(isEmailFieldInvalid, Is.True, "El campo de correo electrónico no es inválido.");
            Assert.That(isMessageFieldInvalid, Is.True, "El campo de mensaje no es inválido.");
        }

        [Test]
        public void testNavigationLinks()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/");

            IWebElement homeLink = driver.FindElement(By.LinkText("Home"));
            homeLink.Click();
            Assert.That(driver.Url, Is.EqualTo("https://localhost:7195/"));

            IWebElement createLink = driver.FindElement(By.LinkText("Create"));
            createLink.Click();
            Assert.That(driver.Url, Is.EqualTo("https://localhost:7195/Product/Create"));

            IWebElement contactLink = driver.FindElement(By.LinkText("Contact"));
            contactLink.Click();
            Assert.That(driver.Url, Is.EqualTo("https://localhost:7195/Product/Contact"));
        }

        [Test]
        public void testContactSuccessMessage()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/Product/Contact");

            IWebElement nameField = driver.FindElement(By.Name("Name"));
            IWebElement emailField = driver.FindElement(By.Name("Email"));
            IWebElement messageField = driver.FindElement(By.Name("Message"));
            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            nameField.SendKeys("Test User");
            emailField.SendKeys("test.user@example.com");
            messageField.SendKeys("This is a test message.");
            submitButton.Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.CssSelector(".alert-success")).Displayed);

            IWebElement successMessage = driver.FindElement(By.CssSelector(".alert-success"));
            Assert.That(successMessage.Text, Is.EqualTo("El formulario de contacto se envió correctamente."));
        }

        [Test]
        public void testDescriptionLengthError()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/Product/Create");

            IWebElement nameField = driver.FindElement(By.Name("Name"));
            IWebElement descriptionField = driver.FindElement(By.Name("Description"));
            IWebElement priceField = driver.FindElement(By.Name("Price"));
            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            nameField.SendKeys("Test Product");
            descriptionField.SendKeys(new string('a', 501));
            priceField.SendKeys("100");
            submitButton.Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.CssSelector("div[style*='background-color: red']")).Displayed);

            IWebElement descriptionError = driver.FindElement(By.CssSelector("div[style*='background-color: red']"));
            Assert.That(descriptionError.Text, Is.EqualTo("La descripción no puede tener más de 500 caracteres."));
        }

        [Test]
        public void testEmptyProduct()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/Product/Create");
            IWebElement nameField = driver.FindElement(By.Name("Name"));
            IWebElement descriptionField = driver.FindElement(By.Name("Description"));
            IWebElement priceField = driver.FindElement(By.Name("Price"));
            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            nameField.Clear();
            descriptionField.Clear();
            priceField.Clear();
            submitButton.Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.CssSelector(":invalid")).Any());

            bool isNameFieldInvalid = nameField.GetAttribute("validationMessage") != null;
            bool isDescriptionFieldInvalid = descriptionField.GetAttribute("validationMessage") != null;
            bool isPriceFieldInvalid = priceField.GetAttribute("validationMessage") != null;

            Assert.That(isNameFieldInvalid, Is.True, "El campo de nombre no es inválido.");
            Assert.That(isDescriptionFieldInvalid, Is.True, "El campo de correo electrónico no es inválido.");
            Assert.That(isPriceFieldInvalid, Is.True, "El campo de mensaje no es inválido.");
        }

        [Test]
        public void testNegativePrice()
        {
            driver.Navigate().GoToUrl("https://localhost:7195/Product/Create");
            IWebElement nameField = driver.FindElement(By.Name("Name"));
            IWebElement descriptionField = driver.FindElement(By.Name("Description"));
            IWebElement priceField = driver.FindElement(By.Name("Price"));
            IWebElement submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));

            nameField.SendKeys("Test Product");
            descriptionField.SendKeys("This is a test product.");
            priceField.SendKeys("-100");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.CssSelector(":invalid")).Any());

            bool isPriceFieldNegative = priceField.GetAttribute("validationMessage") != null;
            Assert.That(isPriceFieldNegative, Is.True, "El campo de precio no es inválido.");
        }

        [Test]
        public void testContactNavigation()
        {
            int numberOfUsers = 5;
            var tasks = new List<Task>();

            for (int i = 0; i < numberOfUsers; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var localDriver = new ChromeDriver();
                    localDriver.Navigate().GoToUrl("https://localhost:7195/Products/Contact");
                    localDriver.Quit();
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }

        [Test]
        public void testProductNavigation()
        {
            int numberOfUsers = 5; // Número de usuarios simulados
            string productPageUrl = "https://localhost:7195/"; // URL de la página principal que muestra los productos
            var tasks = new List<Task>();

            for (int i = 0; i < numberOfUsers; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var localDriver = new ChromeDriver();
                    try
                    {
                        // Navegar a la página de productos
                        localDriver.Navigate().GoToUrl(productPageUrl);

                        // Esperar a que la tabla de productos esté visible
                        var wait = new WebDriverWait(localDriver, TimeSpan.FromSeconds(10));
                        wait.Until(d => d.FindElement(By.CssSelector("table.table tbody tr")).Displayed);

                        // Verificar que haya productos en la tabla
                        var productRows = localDriver.FindElements(By.CssSelector("table.table tbody tr"));
                        Assert.That(productRows.Count, Is.GreaterThan(0), "No se encontraron productos en la tabla.");
                    }
                    finally
                    {
                        // Cerrar el navegador
                        localDriver.Quit();
                    }
                }));
            }

            // Esperar a que todas las tareas se completen
            Task.WaitAll(tasks.ToArray());
        }
    }
}
