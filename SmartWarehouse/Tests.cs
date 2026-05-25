using System;
using System.Collections.Generic;

#nullable enable
public class TestFailureException : Exception
{
    public TestFailureException(string message) : base(message) { }
}
public static class Assert
{
    public static void IsTrue(bool condition, string msg)
    {
        if (!condition) throw new TestFailureException($"[Assert.IsTrue] {msg}");
    }

    public static void IsFalse(bool condition, string msg)
    {
        if (condition) throw new TestFailureException($"[Assert.IsFalse] {msg}");
    }

    public static void AreEqual<T>(T expected, T actual, string msg)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new TestFailureException($"[Assert.AreEqual] {msg} | Expected: {expected}, Actual: {actual}");
    }
}
public static class TaskUnitTests
{
    private static int _passed;
    private static int _failed;
    public static void Run()
    {
        var tests = new Action[]
        {
            Test_Coord_Operators,
            Test_Coord_EqualityAndHashContract,
            Test_Module_CreationAndPolymorphism,
            Test_Engine_EmptyAndNullMaps,
            Test_Engine_ProcessedCountAccuracy,
            Test_Engine_OutAndRefContract,
            Test_SealedClassesCheck
        };
        foreach (var test in tests)
        {
            try
            {
                test();
                _passed++;
                Console.WriteLine($"✅ [PASS] {test.Method.Name}");
            }
            catch (Exception ex)
            {
                _failed++;
                Console.WriteLine($"❌ [FAIL] {test.Method.Name}: {ex.Message}");
            }
        }

        Console.WriteLine($"\n Итог: {_passed} пройдено, {_failed} провалено. Всего: {_passed + _failed}");
    }
    private static void Test_Coord_Operators()
    {
        var c1 = new Coord(2, 5);
        var c2 = new Coord(1, -3);

        Assert.AreEqual(new Coord(3, 2), c1 + c2, "Оператор '+' возвращает неверный результат");
        Assert.AreEqual(new Coord(1, 8), c1 - c2, "Оператор '-' возвращает неверный результат");
        Assert.AreEqual(new Coord(6, 15), c1 * 3, "Оператор '*' (скаляр) возвращает неверный результат");
    }
    private static void Test_Coord_EqualityAndHashContract()
    {
        var a = new Coord(7, 7);
        var b = new Coord(7, 7);
        var c = new Coord(7, 8);
        Assert.IsTrue(a == b, "Оператор '==' должен возвращать true для идентичных координат");
        Assert.IsTrue(a != c, "Оператор '!=' должен возвращать true для разных координат");
        Assert.IsTrue(a.Equals(b), "Метод Equals должен возвращать true для равных структур");
        Assert.IsFalse(a.Equals(null), "Equals(null) должен возвращать false");
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), "Хэш-коды равных структур обязаны совпадать (контракт System.Object)");
    }
    private static void Test_Module_CreationAndPolymorphism()
    {
        AbstractModule scout = new ScoutModule(new Coord(0, 0), 10);
        AbstractModule cargo = new CargoModule(new Coord(5, 5), 10);
        scout.Act();
        cargo.Act();
        Assert.IsTrue(true, "Полиморфный вызов Act() выполнен без исключений");
    }
    private static void Test_Engine_EmptyAndNullMaps()
    {
        var engine = new SimulationEngine();
        int fuel1 = 0;
        bool res1 = engine.TryStep(Array.Empty<AbstractModule[]>(), 0, out int cnt1, ref fuel1);
        Assert.IsFalse(res1, "Пустая карта должна возвращать false");
        Assert.AreEqual(0, cnt1, "Количество обработанных на пустой карте должно быть 0");
        var mapWithNulls = new AbstractModule[2][] { null, new AbstractModule[0] };
        int fuel2 = 0;
        bool res2 = engine.TryStep(mapWithNulls, 0, out int cnt2, ref fuel2);
        Assert.IsFalse(res2, "Карта из null/пустых строк должна возвращать false");
        Assert.AreEqual(0, cnt2, "Количество обработанных должно быть 0");
    }
    private static void Test_Engine_ProcessedCountAccuracy()
    {
        var engine = new SimulationEngine();
        var map = new AbstractModule[3][];
        map[0] = new AbstractModule[] { new ScoutModule(new Coord(0,0), 100), null };
        map[1] = new AbstractModule[] { new CargoModule(new Coord(1,1), 100), new ScoutModule(new Coord(2,2), 100) };
        map[2] = null;

        int fuel = 0;
        bool res = engine.TryStep(map, 1, out int processed, ref fuel);

        Assert.IsTrue(res, "Наличие модулей должно возвращать true");
        Assert.AreEqual(3, processed, "Счётчик processed должен точно соответствовать количеству не-null модулей");
    }
    private static void Test_Engine_OutAndRefContract()
    {
        var engine = new SimulationEngine();
        var map = new AbstractModule[1][] { new AbstractModule[] { new ScoutModule(new Coord(0,0), 10) } };
        
        int outParam = -999;
        int refParam = 50;

        engine.TryStep(map, 0, out outParam, ref refParam);

        Assert.AreEqual(1, outParam, "out-параметр должен быть инициализирован методом (ожидаем 1 модуль)");
        Assert.IsFalse(refParam == 50, "ref-параметр должен быть изменён внутри метода (ожидается != 50)");
    }
    private static void Test_SealedClassesCheck()
    {
        Assert.IsTrue(typeof(ScoutModule).IsSealed, "ScoutModule должен быть объявлен как sealed");
        Assert.IsTrue(typeof(CargoModule).IsSealed, "CargoModule должен быть объявлен как sealed");
        Assert.IsTrue(typeof(AbstractModule).IsAbstract, "AbstractModule должен быть объявлен как abstract");
    }
}

// Пример запуска программы:
// public class Program
// {
//     public static void Main()
//     {
//         TaskUnitTests.Run();
//     }
// }