namespace UnitTests.FSharp.Configuration

open System
open NUnit.Framework
open Arrow.FSharp.Configuration

type Person() =
    member val Name = "" with get, set
    member val Age = 0 with get, set

[<TestFixture>]
type AppConfigTests() =
    [<Test>]
    member this.``Get missing section``() =
        AppConfig.getSectionXml "Test" "Foo" |> isNothing

    [<Test>]
    member this.``Get section``() =
        AppConfig.getSectionXml "Test" "People" |> isSomething

    [<Test>]
    member this.``Get section via xpath``() =
        AppConfig.getSectionXml "Test" "People/Default" |> isSomething

    [<Test>]
    member this.``Get section object``() =
        match AppConfig.getSectionObject<Person> "Test" "People/Default" with
        | None -> isFail()
        | Some v -> 
            v.Name |> isEqualTo "Jack"
            v.Age |> isEqualTo 20

    [<Test>]
    member this.``Get section object not present``() =
        AppConfig.getSectionObject<Person> "Test" "People/Leader" |> isNothing

    [<Test>]
    member this.``Get section objects``() =
        let persons = AppConfig.getSectionObjects<Person> "Test" "Team" "Person"
        persons |> listHasItems
        List.length persons |> isEqualTo 2

        let a = List.toArray persons
        a.[0].Name |> isEqualTo "Ben"
        a.[0].Age |> isEqualTo 40

        a.[1].Name |> isEqualTo "Sawyer"
        a.[1].Age |> isEqualTo 35

