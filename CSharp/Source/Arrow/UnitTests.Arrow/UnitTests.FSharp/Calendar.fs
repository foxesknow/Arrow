namespace UnitTests.FSharp.Calendar

open System
open NUnit.Framework
open Arrow.FSharp.Calendar

module Tests =
    [<TestFixture>]
    type DateTests() = 
        [<Test>]
        member this.``Next weekday from Thursday``() =
            // A Friday
            let date = Date.makeDate 2018 2 1 |> Date.nextWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Friday))

        [<Test>]
        member this.``Next weekday from Friday``() =
            // A Friday
            let date = Date.makeDate 2018 2 2 |> Date.nextWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Monday))

        [<Test>]
        member this.``Next weekday from Saturday``() =
            // A Friday
            let date = Date.makeDate 2018 2 3 |> Date.nextWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Monday))

        [<Test>]
        member this.``Next weekday from Sunday``() =
            // A Friday
            let date = Date.makeDate 2018 2 4 |> Date.nextWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Monday))


        [<Test>]
        member this.``Previous weekday from Tuesday``() =
            // A Friday
            let date = Date.makeDate 2018 2 6 |> Date.previousWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Monday))

        [<Test>]
        member this.``Previous weekday from Monday``() =
            // A Friday
            let date = Date.makeDate 2018 2 5 |> Date.previousWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Friday))

        [<Test>]
        member this.``Previous weekday from Sunday``() =
            // A Friday
            let date = Date.makeDate 2018 2 4 |> Date.previousWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Friday))

        [<Test>]
        member this.``Previous weekday from Saturday``() =
            // A Friday
            let date = Date.makeDate 2018 2 3 |> Date.previousWeekDay
            Assert.That(date.DayOfWeek, Is.EqualTo(DayOfWeek.Friday))

        [<Test>]
        member this.``First of February``() =
            let date = Date.makeDate 2018 2 15 |> Date.firstOfMonth
            Assert.That(date.Day, Is.EqualTo(1))
            Assert.That(date.Month, Is.EqualTo(2))
            Assert.That(date.Year, Is.EqualTo(2018))

        [<Test>]
        member this.``Last of February``() =
            let date = Date.makeDate 2018 2 15 |> Date.lastOfMonth
            Assert.That(date.Day, Is.EqualTo(28))
            Assert.That(date.Month, Is.EqualTo(2))
            Assert.That(date.Year, Is.EqualTo(2018))

        [<Test>]
        member this.``Last of March``() =
            let date = Date.makeDate 2018 3 5 |> Date.lastOfMonth
            Assert.That(date.Day, Is.EqualTo(31))
            Assert.That(date.Month, Is.EqualTo(3))
            Assert.That(date.Year, Is.EqualTo(2018))

