![JsonEasyNavigation](./media/logo.png "Mass Transit")

# JsonEasyNavigation

This library provides a wrapper class around JsonElement (located in System.Text.Json) which allows to navigate through JSON DOM (domain object model) hierarchy using indexer-style syntax (as in collections and dictionaries) for properties and array alike. It also contains useful methods to get values without throwing exceptions.

Here is an example:

```JSON
{
    "Persons": [
        {
            "Id": 0,
            "Name": "John",
            "SecondName": "Wick",
            "NickName": "Baba Yaga",
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

Now we can easly navigate Domain Object Model using indexers in a sequential style:

```C#
var arrayItem = nav[0]; // first item in the array
var id = arrayItem["Id"].GetInt32OrDefault(); // 0
var nickName = arrayItem["NickName"].GetStringOrDefault(); // "Baba Yaga"
```

Notice the usage of `GetXxxOrDefault` methods, which provides a convinient way to get values from the JsonElement without throwing exceptions. There are a lot of other similar useful methods.

We also can check if the property exist:

```C#
if (nav[0]["Age"].Exist)
{
    // Do something if the Age property of the first object in array exist.
}
```

`JsonNavigationElement` does **not** throw exception if a property or array item does not exist. You can always check `Exist` property of an `JsonNavigationElement` to be sure that corresponding `JsonElement` was found.

## Features

Overall, the library provides following features:

* A wrapper around JsonElement incapsulating all behaviour regarding to the DOM traversial and navigation (`JsonNavigationElement`);
* The API is implemented in a no-throw manner - you can "get" properties that don't exist in the DOM and check their existinence;
* Implementation of a `IReadOnlyDictionary` and `IReadOnlyCollection`;
* Methods for converting values to the specified types in a type-safe way (and also generic methods like `TryGetValue<T>`);
* Extensions for caching properties and persisting their order for faster and easier JSON navigation.

## Contributing

If you would like to fix some bugs and add new features (in a non-breaking manner) you are free to send pull-requests.

## License

This software is distributed under the Apache License 2.0. See LICENSE.txt.

