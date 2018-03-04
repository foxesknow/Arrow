namespace Arrow.FSharp.Calendar

open System

module Clock =
    /// Returns the current local time
    let now = 
        Arrow.Calendar.Clock.Now

    /// Returns the current UTC time
    let utcNow = 
        Arrow.Calendar.Clock.UtcNow
        


module Date =
    /// Returns the next week day
    let nextWeekDay (date : DateTime) =
        match date.DayOfWeek with
        | DayOfWeek.Friday -> date.AddDays(3.0)
        | DayOfWeek.Saturday -> date.AddDays(2.0)
        | _ -> date.AddDays(1.0)

    /// Returns the previous week day
    let previousWeekDay (date : DateTime) =
        match date.DayOfWeek with
        | DayOfWeek.Monday -> date.AddDays(-3.0)
        | DayOfWeek.Sunday -> date.AddDays(-2.0)
        | _ -> date.AddDays(-1.0)

    /// Returns the first of the specified month
    let firstOfMonth (date : DateTime) =
        let delta = date.Day - 1
        date.AddDays(-float(delta))

    let lastOfMonth (date : DateTime) =
        let first = firstOfMonth date
        let firstOfNextMonth = first.AddMonths(1)
        firstOfNextMonth.AddDays(-1.0)

    /// Creates a new date
    let makeDate year month day =
        DateTime(year, month, day)

