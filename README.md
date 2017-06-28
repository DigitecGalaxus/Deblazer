# Deblazer ORM
![Build Status](https://tkae.visualstudio.com/_apis/public/build/definitions/600337e5-0517-476a-aa93-a7831c02c8cc/5/badge)
[![Build Status](https://travis-ci.org/DigitecGalaxus/Deblazer.svg?branch=master)](https://travis-ci.org/DigitecGalaxus/Deblazer)

Deblazer is the C# ORM written and used by [Digitec Galaxus](https://github.com/DigitecGalaxus) it has query language similiar to LINQ.

## Getting Started
Install the latest package from Nuget [<insert latest release url here>](https://gogole.ch)

### Usuage
Use the [artifacts tooling](https://github.com/DigitecGalaxus/Deblazer.Artifacts) to generate the necessary artifacts classes.
```cs
var db = new Db("My Connection String");

var top10people = db.Application_Peoples()
    .TakeDb(10)
    .ToList();
```

## Development
Deblazer is written in C# and targets the .NET 4.6.1 Framework
