Diffmark
========

Diffmark is a DSL (Domain-Specific Language) for transforming one string to another.

The main purpose of this language is to reduce redundancies in large sets of related strings.
One obvious example of this is dictionaries. Suppose you have a list of nouns that show both their singular and plural forms:
```
> fireman/firemen
> king penguin/king penguins
> embolus/emboli
...
```
Both forms are most often almost identical; re-entering most of the same string is both messy and a waste of space.
Let's rewrite the same three nouns with Diffmark patterns that change the singular form to the plural form:
```
> fireman/--en
> king penguin/+s
> embolus/--i
```
In the Diffmark library, these string/pattern pairs can be processed with a simple line of code to get the plural form:
```cs
string plural = Diff.Mark("fireman", "--en"); // "firemen"
```

##Syntax
A string with no symbols is just added to the end of the base string.

> `Hello` + `\sworld` = `Hello world`

###`\` (Escape)
Escapes the next character. Example: `\+` -> `+`

And there are some special escape sequences, too:
* `\n`: Line feed
* `\r`: Carriage return
* `\s`: Space
* `\t`: Tab

###`+` (Add)
Adds a string to the start or end of a string.
The `+` is optional for appending.
You can put it there if you really want to.

> `finch` + `gold+` = `goldfinch`

> `orange` + `+s` = `oranges`

###`-` (Delete)
Deletes a character from the start or end of the base string.
Multiple `-` symbols delete more characters.

> `escape` + `-ing` = `escaping`

> `trapped` + `sn--` = `snapped`

###`*` (Replace word)
Replaces a word at the start or end of the base string.

> `I like fixing bugs.` + `HATE**` = `I HATE fixing bugs.`

> `black bear` + `*hole` = `black hole`

###`;` (Statement separator)
Separate distinct patterns to apply several consecutive transformations.

> `love` + `I + ; + Diffmark!` = `I love Diffmark!`

##License
Diffmark is provided under the permissive and glorious [MIT License](https://github.com/TheBerkin/Diffmark/blob/master/LICENSE)!

That means you can diff your marks all day long, free of charge.
