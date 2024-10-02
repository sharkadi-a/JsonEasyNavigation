# JsonEasyNavigation
This library provides a wrapper class around JsonElement (located in System.Text.Json) which allows to navigate through JSON DOM (domain object model) hierarchy using indexer-style syntax (as in collections and dictionaries) for properties and array alike. It also contains useful methods to get values without throwing exceptions.


---


<table>
<tr>
<th></th>
<th>Description</th>
</tr>
<tr>
<td>UPDATED:</td>
<td>9/17/2024</td>
</tr>
<tr>
<td>FRAMEWORK:</td>
<td>netstandard2.0, netstandard2.1, net9.0</td>
</tr>
<tr>
<td>LANGUAGE:</td>
<td>[C#] preview</td>
</tr>
<tr>
<td>OUTPUT TYPE:</td>
<td>Library [API]</td>
</tr>
<tr>
<td>SUPPORTS:</td>
<td>[Visual Studio]</td>
</tr>
<tr>
<td>GFX SUBSYS:</td>
<td>[None]</td>
</tr>
<tr>
<td>TAGS:</td>
<td>[text.json json navigation indexer dictionary list collection DOM]</td>
</tr>
<tr>
<td>STATUS:</td>
<td><a href="https://github.com/Latency/JsonEasyNavigation/actions/workflows/dotnet.yml"><img src="https://github.com/Latency/JsonEasyNavigation/actions/workflows/dotnet.yml/badge.svg"></a></td>
</tr>
<tr>
<td>LICENSE:</td>
<td><a href="https://github.com/Latency/JsonEasyNavigation/blob/master/MIT-LICENSE.txt"><img src="https://img.shields.io/github/license/Latency/JsonEasyNavigation?style=plastic&logo=GitHub&logoColor=black&label=License&color=yellowgreen"></a></td>
</tr>
<tr>
<td>VERSION:</td>
<td><a href="https://github.com/Latency/JsonEasyNavigation/releases"><img src="https://img.shields.io/github/v/release/Latency/JsonEasyNavigation?include_prereleases&style=plastic&logo=GitHub&logoColor=black&label=Version&color=blue"></a></td>
</tr>
<!-- VERSION: 1.3.2 -->
</table>


![JsonEasyNavigation](media/logo.png "JsonEasyNavigation")

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

This software is distributed under the Apache License 2.0. See [LICENSE].txt.

All graphical assets are licensed under the [Creative Commons Attribution 3.0 Unported License](https://creativecommons.org/licenses/by/3.0/).

[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job.)

   [Creative Commons Attribution 3.0 Unported License]: <https://creativecommons.org/licenses/by/3.0/>
   [LICENSE]: <https://www.apache.org/licenses/LICENSE-2.0>
   [MSDN article]: <https://msdn.microsoft.com/en-us/library/c5b8a8f9(v=vs.100).aspx>
