[<AutoOpen>]
module NUnitWrappers

open NUnit.Framework

let isEqualTo (lhs : 'T1) (rhs : 'T2) =
    Assert.That(lhs, Is.EqualTo(rhs))

let isNotEqualTo (lhs : 'T1) (rhs : 'T2) =
    Assert.That(lhs, Is.Not.EqualTo(rhs))

let isTrue (b : bool) =
    Assert.IsTrue(b)

let isFalse (b : bool) =
    Assert.IsFalse(b)

let isFail() =
    Assert.Fail()

let isPass() =
    Assert.Pass();

let isSomething option =
    match option with
    | Some _ -> isPass()
    | None -> isFail()

let isNothing option =
    match option with
    | Some _ -> isFail()
    | None -> isPass()

let listHasItems list =
    match list with
    | [] -> isFail()
    | head :: tail -> isPass()

let isEmpty (s : seq<_>) =
    if Seq.isEmpty s then
        isPass()
    else
        isFail()

let isNotEmpty (s : seq<_>) =
    if Seq.isEmpty s then
        isFail()
    else
        isPass()

let isNotNull (item : obj) =
    match item with
    | null -> isFail()
    | _ -> isPass()