![JsonEasyNavigation](src/Resources/project.png "JsonEasyNavigation")

<hr>

## Navigation
* <a href="#introduction">Introduction</a>
* <a href="#usage">Usage</a>
* <a href="#features">Features</a>
* <a href="#installation">Installation</a>
* <a href="#license">License</a>

<hr>

<h2><a name="introduction">Introduction</a></h2>

This library provides a wrapper class around Microsoft's .NET JsonElement JsonElement (located in System.Text.Json) which allows to navigate through JSON DOM (domain object model) hierarchy using indexer-style syntax (as in collections and dictionaries) for properties and array alike. It also contains useful methods to get values without throwing exceptions. 
Target frameworks are .NET 5 and NET Standard 2.0.

<h2><a name="usage">Usage</a></h2>

Here is an example:

```JSON
{
    "Persons": [
        {
            "Id": 0,
            "Name": "John",
            "SecondName": "Wick",
            "NickName": "Baba Yaga"
        },
        {
            "Id": 1,
            "Name": "Wade",
            "SecondName": "Winston",
            "NickName": "Deadpool"
        }
    ]
}
```

Assume that we are using `System.Text.Json` so we can create JsonDocument:

```C#
var jsonDocument = JsonDocument.Parse(json);
```

Then we convert this JSON document to the `JsonNavigationElement` provided by JsonEasyNavigation library:

```C#
var nav = jsonDocument.ToNavigation();
```

`JsonNavigationElement` is a struct, a wrapper around JsonElement. This struct provides many useful methods to operate arrays, objects and getting values from the JsonElement inside.

Now we can easley navigate Domain Object Model using indexers in a sequential style:

```C#
var arrayItem = nav[0]; // first item in the array
var id = arrayItem["Id"].GetInt32OrDefault(); // 0
var nickName = arrayItem["NickName"].GetStringOrDefault(); // "Baba Yaga"
```

Notice the usage of `GetXxxOrDefault` methods, which provides a convenient way to get values from the JsonElement without throwing exceptions. There are a lot of other similar useful methods.

We also can check if the property exist:

```C#
if (nav[0]["Age"].Exist)
{
    // Do something if the Age property of the first object in array exist.
}
```

`JsonNavigationElement` does **not** throw exception if a property or array item does not exist. You can always check `Exist` property of an `JsonNavigationElement` to be sure that corresponding `JsonElement` was found.

It is also possible to map `JsonNavigationElement` into the object of the specific type (using JsonSerializer internally):

```C#
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SecondName { get; set; }
    public string NickName { get; set; } 
}

// ...

var person = nav[0].Map<Person>();
```

<h2><a name="features">Features</a></h2>

Overall, the library provides following features:

* A wrapper around JsonElement encapsulating all behaviour regarding to the DOM traversal and navigation (`JsonNavigationElement`);
* The API is implemented in a no-throw manner - you can "get" properties that don't exist in the DOM and check their existence;
* Implementation of a `IReadOnlyDictionary` and `IReadOnlyCollection`;
* Methods for converting values to the specified types in a type-safe way (and also generic methods like `TryGetValue<T>`);
* Extensions for caching properties and persisting their order for faster and easier JSON navigation.

<h2><a name="installation">Installation</a></h2>

This library can be installed using NuGet found [here](https://www.nuget.org/packages/JsonEasyNavigation/).
This library can be installed using GitHub found [here](https://github.com/users/Latency/packages/nuget/package/JsonEasyNavigation).

<h2><a name="license">License</a></h2>

The source code for the site is licensed under the MIT license, which you can find in
the [MIT-LICENSE].txt file.

All graphical assets are licensed under the
[Creative Commons Attribution 3.0 Unported License](https://creativecommons.org/licenses/by/3.0/).

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job.)

   [GNU LESSER GENERAL PUBLIC LICENSE]: <http://www.gnu.org/licenses/lgpl-3.0.en.html>
   [MSDN article]: <https://msdn.microsoft.com/en-us/library/c5b8a8f9(v=vs.100).aspx>
   [MIT-License]: <http://choosealicense.com/licenses/mit/>