namespace Arrow.FSharp.Configuration

open Arrow.Configuration
open Arrow.Xml.ObjectCreation;

module AppConfig = 
    /// Reads a section from the config file as xml
    let getSectionXml system section =
        match AppConfig.GetSectionXml(system, section) with
        | null -> None
        | v -> Some(v)

    /// Tries to create on object from xml
    let tryGetSectionObject<'T> system section =
        match AppConfig.GetSectionXml(system, section) with
        | null -> None
        | xml -> Some(XmlCreation.Create<'T>(xml))

    /// Creates an object from a section
    let getSectionObject<'T when 'T :not struct and 'T :null> system section =
        match AppConfig.GetSectionObject<'T>(system, section) with
        | null -> None
        | v -> Some(v)

    /// Create a list of object held in a section
    let getSectionObjects<'T when 'T :not struct and 'T :null> system section objectElementNames =
        let items =AppConfig.GetSectionObjects<'T>(system, section, objectElementNames)
        Seq.toList items