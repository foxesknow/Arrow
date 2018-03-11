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
    /// Add days
    let addDays days (date : DateTime) =
        date.AddDays(days)

    /// Adds months
    let addMonths months (date : DateTime) =
        date.AddMonths(months)

    /// Adds years
    let addYears years (date : DateTime) =
        date.AddYears(years)

    let day (date : DateTime) =
        date.Day

    let month (date : DateTime) =
        date.Month

    let year (date : DateTime) =
        date.Year

    let dayOfWeek (date : DateTime) =
        date.DayOfWeek

    /// Extracts the date part 
    let extractDate (date: DateTime) =
        date.Date

    /// Extracts the time part
    let extractTime (date : DateTime) =
        date.TimeOfDay

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

    /// Returns the last day of the month
    let lastOfMonth (date : DateTime) =
        date |> firstOfMonth |> addMonths 1 |> addDays -1.0
        
    /// Creates a new date
    let makeDate year month day =
        DateTime(year, month, day)


module Time =
    let makeTime hours minutes seconds =
        TimeSpan(hours, minutes, seconds)

    let fromDays days =
        TimeSpan.FromDays(days)

    let fromHours hours =
        TimeSpan.FromHours(hours)

    let fromMinutes minutes=
        TimeSpan.FromMinutes(minutes)

    let fromSeconds seconds =
        TimeSpan.FromSeconds(seconds)

    let fromMilliseconds milliseconds =
        TimeSpan.FromMilliseconds(milliseconds)

    let add (x : TimeSpan) (y : TimeSpan) =
        x.Add(y)

    let negate (x : TimeSpan) =
        x.Negate()