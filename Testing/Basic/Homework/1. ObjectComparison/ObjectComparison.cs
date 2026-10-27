using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;

public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        actualTsar.Should().BeEquivalentTo(expectedTsar, config => config.Excluding((IMemberInfo x) => x.Name == "Id"));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 


        // я думаю, что недостатки у этого подхода следующие:
        // 1) при появлении новых полей придётся дописывать то страшненькое выражение, которое содержится в Return.
        // Если этого не сделать, то такие новые поля тестом будут проигнорированы
        // 2) код нечитаем: первое выражение проверяет, указывают ли actual, expected на один и тот же объект,
        // либо равны ли они null, что может быть не ясно из такой записи, а так же страшное выражение в return довольно объёмное,
        // громоздкое и, главное, вводящее проверяемые поля вручную.
        // Я этого избежал, использовав FluentAssertions:
        // метод расширения обходит графы двух сравниваемых объектов и сравнивает поля и вложенные объекты повершинно,
        // рядом же прописано, какие поля нужно исключить из сравнения. Добавление нового поля, если его не нужно исключать из сравнения, ведёт к минимуму изменений в коде.
        // Наконец, ClassicAssert.True() при падении теста напишет Expected True But Was False,
        // а FluentAssertions напишет, что, где сломалось и по какой причине
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
