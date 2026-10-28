using NUnit.Framework;

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
    
    // благодаря выводу сообщения, если написать некорректный тест(например, в первом тесте написать +1222.23), в тексте ошибки будут выведены данные теста: value: +1222.23, precision: 4, scale: 2, onlyPositive: True
    
    [TestCase("+1.23", 4, 2, true, TestName = "Знак плюса учитывается в подсчёте длины для precision")]
    [TestCase("1.23", 3, 2, true, TestName = "Число, имеющее 2 знака после запятой и 1 знак в целой части, имеет precision:3 и scale:2")]
    [TestCase("1,23", 3, 2, true, TestName = "Разделителем может быть и запятая")]
    [TestCase("0.0", 2, 1, true, TestName = "Несмотря на то, что по правилам математики 0.0 = 0, согласно спецификации из налоговой 0.0 - число с precision:2 и scale:1")]
    [TestCase("0", 1, 0, true, TestName = "Число без дробной части имеет scale = 0 и precision, равный длине этого числа")]
    [TestCase("-123", 4, 0, false, TestName = "Знак минуса учитывается в длине числа")]
    [TestCase("-1.23", 4, 2, false, TestName = "Знак минуса учитывается в длине числа, даже когда у него есть дробая часть")]
    public void NumberValidator_IsValidNumber_TypicalInput_ReturnsTrue(string value, int precision, int scale = 0, bool onlyPositive = true)
    {
        Assert.That(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value), Is.True, $"value: {value}, precision: {precision}, scale: {scale}, onlyPositive: {onlyPositive}");
    }

    [TestCase("0.000", 17, 2, true, TestName = "scale не допускает точность больше заданной")]
    [TestCase("a.sd", 3, 2, true, TestName = "буквы вместо цифр недопустимы")]
    [TestCase("+1.23", 3, 2, true, TestName = "precision учитывает знак плюс при подсчёте длины целой части числа")]
    [TestCase("-1.23", 3, 2, true, TestName = "precision учитывает знак минуса при подсчёте длины целой части числа")]
    [TestCase("00.00", 3, 2, true, TestName = "если общая длина(длина целой части и длина дробной части) числа больше чем precision, проверку оно не проходит")]
    [TestCase("111", 2, 0, true, TestName = "если длина целого числа больше precision, проверку такое число не проходит")]
    [TestCase("-0.00", 3, 2, true, TestName = "Несмотря на то, что по правилам математики -0.00 = 0, согласно спецификации из налоговой это число с precision:4 и scale:2")]
    [TestCase("+0.00", 3, 2, true, TestName = "Несмотря на то, что по правилам математики +0.00 = 0, согласно спецификации из налоговой это число с precision:4 и scale:2")]
    [TestCase("-1", 1, 0, true, TestName = "при флаге onlyPositive отрицательные числа не проходят проверку")]
    [TestCase("+", 1, 0, true, TestName = "Знак плюса без цифр не проходит проверку")]
    [TestCase("-", 1, 0, true, TestName = "Знак минуса без цифр не проходит проверку")]
    [TestCase(".", 2, 1, true, TestName = "Разделитель(точка) без цифр не проходит проверку")]
    [TestCase(",", 2, 1, true, TestName = "Разделитель(запятая) без цифр не проходит проверку")]
    [TestCase("", 1, 0, true, TestName = "Пустая строка не является числом и не проходит проверку")]
    [TestCase(null, 1, 0, true, TestName = "null не проходит проверку")]
    [TestCase(" ", 1, 0, true, TestName = "Строка из пробела не является числом")]
    [TestCase("1.", 2, 1, true, TestName = "Число с разделителем без дробной части не проходит проверку")]
    [TestCase(".1", 2, 1, true, TestName = "Число с разделителем без целой части не проходит проверку")]
    [TestCase("1.1.1", 3, 2, true, TestName = "Число с двумя разделителями не проходит проверку")]
    [TestCase("++1", 3, 0, true, TestName = "Число с двумя знаками не проходит проверку")]
    [TestCase("1+", 2, 0, true, TestName = "Число со знаком после такого числа не проходит проверку")]
    [TestCase(" 1", 2, 0, true, TestName = "Число с пробелом до этого числа не проходит проверку")]
    [TestCase("1 ", 1, 0, true, TestName = "Число с пробелом после этого числа не проходит проверку")]
    public void NumberValidator_IsValidNumber_InvalidInput_ReturnsFalse(string value, int precision, int scale = 0,
        bool onlyPositive = true)
    {
        Assert.That(new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value), Is.False, $"value: {value}, precision: {precision}, scale: {scale}, onlyPositive: {onlyPositive}");
    }
}