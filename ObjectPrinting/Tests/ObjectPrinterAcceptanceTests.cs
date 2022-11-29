﻿using System;
using System.Globalization;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private PrintingConfig<Person> printingConfig;
        private readonly Person person = new Person { Name = "Alex", Surname = "Tsvetkov", Age = 19, Height = 5.6, Width = 7.2f };

        [SetUp]
        public void SetUp()
        {
            printingConfig = ObjectPrinter.For<Person>();
        }
        
        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //6. Исключить из сериализации конкретного свойства
                .Excluding(x => x.Age)
                //4. Настроить сериализацию конкретного свойства
                .Printing(x => x.Name).Using(x => x.ToString() + "AAA")
                //3. Для числовых типов указать культуру
                .Printing<float>().Using(CultureInfo.InvariantCulture)
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<double>().Using(x => (x + 100).ToString())
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(x => x.Name).TrimmedToLength(2);

                var s1 = printer.PrintToString(person);
                //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
                var s2 = person.PrintToString();
                //8. ...с конфигурированием
                var s3 = person.PrintToString(x => x.Excluding(x => x.Age));
        }

        [Test]
        public void PrintingConfig_WithExcludeType_ShouldReturnString()
        {
            var personConfig = printingConfig.Excluding<Guid>();

            var result = personConfig.PrintToString(person);
            
            Assert.That(result, Has.No.Contains("Id"));
        }
        
        [Test]
        public void PrintingConfig_WithExcludeProperty_ShouldReturnString()
        {
            var personConfig = printingConfig.Excluding(x => x.Age);

            var result = personConfig.PrintToString(person);
            
            Assert.That(result, Has.No.Contains("Age"));
        }
        
        [Test]
        public void PrintingConfig_WithCustomSerializeType_ShouldReturnString()
        {
            var personConfig = printingConfig.Printing<Guid>().Using(x => "id111");

            var result = personConfig.PrintToString(person);
            
            Assert.That(result, Has.No.Contains("Guid"));
        }
    }
}