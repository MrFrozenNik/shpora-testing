using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1, -1)]
    [TestCase(1, 2)]
    [TestCase(1, 1)]
    [TestCase(-1, 2)]
    public void NumberValidator_InvalidArguments_Throw(int precision, int scale = 0)
    {
        Assert.That(() => new NumberValidator(precision, scale), Throws.TypeOf<ArgumentException>(), $"precision: {precision}, scale: {scale}");
    }
    
    [TestCase(1)]
    [TestCase(2, 1)]
    [TestCase(17, 2)]
    public void NumberValidator_ValidArguments_DontThrow(int precision, int scale = 0)
    {
        Assert.That(() => new NumberValidator(precision, scale), Throws.Nothing, $"precision: {precision}, scale: {scale}");

    }
    
    
    [Test]
    public void Test()
    {
        Assert.Multiple(() =>
        {

            Assert.That(new NumberValidator(4, 2, true).IsValidNumber("+1.23"), Is.True, "");
            Assert.That(new NumberValidator(17, 2, true).IsValidNumber("0.0"), Is.True, "");
            Assert.That(new NumberValidator(17, 2, true).IsValidNumber("0"), Is.True, "");
            // scale не допускает точность больше заданной
            Assert.That(new NumberValidator(17, 2, true).IsValidNumber("0.000"), Is.False, "");

            //буквы вместо цифр недопустимы
            Assert.That(new NumberValidator(3, 2, true).IsValidNumber("a.sd"), Is.False, "");

            // precision учитывает знаки плюса и минуса как часть длины целой части числа.
            Assert.That(new NumberValidator(3, 2, true).IsValidNumber("+1.23"), Is.False, "");
            Assert.That(new NumberValidator(3, 2, true).IsValidNumber("-1.23"), Is.False, "");
            //подгруппа: несмотря на то, что по правилам математики 00.00, -0, +0 - одно и то же, согласно спецификации налогового органа это разные числа, не подходящие под регулярку
            Assert.That(new NumberValidator(3, 2, true).IsValidNumber("00.00"), Is.False, "");
            Assert.That(new NumberValidator(3, 2, true).IsValidNumber("-0.00"), Is.False, "");
            Assert.That(new NumberValidator(3, 2, true).IsValidNumber("+0.00"), Is.False, "");
        });
    }
}