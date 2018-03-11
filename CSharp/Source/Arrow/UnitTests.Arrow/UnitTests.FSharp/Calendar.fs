namespace UnitTests.FSharp.Calendar

open System
open NUnit.Framework
open Arrow.FSharp.Calendar

[<TestFixture>]
type DateTests() = 
    [<Test>]
    member this.``Adding days``() =
        Date.makeDate 2018 2 2 |> Date.addDays 1.0 |> Date.day |> isEqualTo 3
        Date.makeDate 2018 2 2 |> Date.addDays -1.0 |> Date.day |> isEqualTo 1
        Date.makeDate 2018 2 1 |> Date.addDays -1.0 |> Date.day |> isEqualTo 31

    [<Test>]
    member this.``Next weekday from Thursday``() =
        let date = Date.makeDate 2018 2 1 |> Date.nextWeekDay
        date.DayOfWeek |> isEqualTo DayOfWeek.Friday

    [<Test>]
    member this.``Next weekday from Friday``() =
        Date.makeDate 2018 2 2 |> Date.nextWeekDay |> Date.dayOfWeek |> isEqualTo DayOfWeek.Monday

    [<Test>]
    member this.``Next weekday from Saturday``() =
        Date.makeDate 2018 2 3 |> Date.nextWeekDay |> Date.dayOfWeek |> isEqualTo DayOfWeek.Monday      

    [<Test>]
    member this.``Next weekday from Sunday``() =
        Date.makeDate 2018 2 4 |> Date.nextWeekDay |> Date.dayOfWeek |> isEqualTo DayOfWeek.Monday

    [<Test>]
    member this.``Previous weekday from Tuesday``() =
        Date.makeDate 2018 2 6 |> Date.previousWeekDay |> Date.dayOfWeek |> isEqualTo DayOfWeek.Monday

    [<Test>]
    member this.``Previous weekday from Monday``() =
        Date.makeDate 2018 2 5 |> Date.previousWeekDay |> Date.dayOfWeek |> isEqualTo DayOfWeek.Friday

    [<Test>]
    member this.``Previous weekday from Sunday``() =
         Date.makeDate 2018 2 4 |> Date.previousWeekDay |> Date.dayOfWeek |> isEqualTo DayOfWeek.Friday

    [<Test>]
    member this.``Previous weekday from Saturday``() =
        Date.makeDate 2018 2 3 |> Date.previousWeekDay |> Date.dayOfWeek |> isEqualTo DayOfWeek.Friday

    [<Test>]
    member this.``First of February``() =
        let date = Date.makeDate 2018 2 15 |> Date.firstOfMonth
        date |> Date.day |> isEqualTo 1
        date |> Date.month |> isEqualTo 2
        date |> Date.year |> isEqualTo 2018

    [<Test>]
    member this.``Last of February``() =
        let date = Date.makeDate 2018 2 15 |> Date.lastOfMonth
        date |> Date.day |> isEqualTo 28
        date |> Date.month |> isEqualTo 2
        date |> Date.year |> isEqualTo 2018

    [<Test>]
    member this.``Last of March``() =
        let date = Date.makeDate 2018 3 5 |> Date.lastOfMonth
        date |> Date.day |> isEqualTo 31
        date |> Date.month |> isEqualTo 3
        date |> Date.year |> isEqualTo 2018

