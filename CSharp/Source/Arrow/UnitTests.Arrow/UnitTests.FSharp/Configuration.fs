namespace UnitTests.FSharp.Configuration

open System
open NUnit.Framework
open Arrow.FSharp.Configuration

module Tests =
    type Person() =
        member val Name = "" with get, set
        member val Age = 0 with get, set

    [<TestFixture>]
    type AppConfigTests() =
        [<Test>]
        member this.``Get missing section``() =
            match AppConfig.getSectionXml "Test" "Foo" with
            | None -> Assert.Pass()
            | _ -> Assert.Fail()

        [<Test>]
        member this.``Get section``() =
            match AppConfig.getSectionXml "Test" "People" with
            | None -> Assert.Fail()
            | _ -> Assert.Pass()

        [<Test>]
        member this.``Get section via xpath``() =
            match AppConfig.getSectionXml "Test" "People/Default" with
            | None -> Assert.Fail()
            | _ -> Assert.Pass()

        [<Test>]
        member this.``Get section object``() =
            match AppConfig.getSectionObject<Person> "Test" "People/Default" with
            | None -> Assert.Fail()
            | Some v -> 
                Assert.That(v.Name, Is.EqualTo("Jack"))
                Assert.That(v.Age, Is.EqualTo(20))

        [<Test>]
        member this.``Get section object not present``() =
            match AppConfig.getSectionObject<Person> "Test" "People/Leader" with
            | None -> Assert.Pass()
            | Some v ->  Assert.Fail()

        [<Test>]
        member this.``Get section objects``() =
            let persons = AppConfig.getSectionObjects<Person> "Test" "Team" "Person"
            Assert.That(persons, Is.Not.Null)
            Assert.That((List.length persons), Is.EqualTo(2))

            let a = List.toArray persons
            Assert.That(a.[0].Name, Is.EqualTo("Ben"))
            Assert.That(a.[0].Age, Is.EqualTo(40))

            Assert.That(a.[1].Name, Is.EqualTo("Sawyer"))
            Assert.That(a.[1].Age, Is.EqualTo(35))
